using ERP.Purchasing.Infrastructure.Persistence;
using ERP.Purchasing.Infrastructure.Persistence.Entities;
using ERP.SharedKernel.Abstractions;
using ERP.SharedKernel.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ERP.Purchasing.Infrastructure.Services;
public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly PurchasingDbContext _context;
    private readonly ILogger<DomainEventDispatcher> _logger;

    public DomainEventDispatcher(
        IServiceProvider serviceProvider,
        PurchasingDbContext context,
        ILogger<DomainEventDispatcher> logger)
    {
        _serviceProvider = serviceProvider;
        _context = context;
        _logger = logger;
    }

    public async Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var isProcessed = await _context.ProcessedEvents
            .AnyAsync(e => e.EventId == domainEvent.EventId, cancellationToken);

        if (isProcessed)
        {
            _logger.LogInformation("Event {EventId} already processed, skipping", domainEvent.EventId);
            return;
        }

        _logger.LogInformation("Dispatching domain event: {EventType}", domainEvent.GetType().Name);

        var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
        var handlers = _serviceProvider.GetServices(handlerType);

        foreach (var handler in handlers)
        {
            try
            {
                var handleMethod = handlerType.GetMethod(nameof(IDomainEventHandler<IDomainEvent>.HandleAsync));
                await (Task)handleMethod.Invoke(handler, new object[] { domainEvent, cancellationToken });

                _logger.LogInformation("Successfully dispatched event to handler: {HandlerType}",
                    handler.GetType().Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error dispatching event {EventType} to handler {HandlerType}",
                    domainEvent.GetType().Name, handler.GetType().Name);
                throw;
            }
        }

        // Mark as processed
        await _context.ProcessedEvents.AddAsync(new ProcessedEvent
        {
            EventId = domainEvent.EventId,
            ProcessedAt = DateTime.UtcNow,
            EventType = domainEvent.GetType().Name
        }, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        foreach (var domainEvent in domainEvents)
        {
            await DispatchAsync(domainEvent, cancellationToken);
        }
    }
}
