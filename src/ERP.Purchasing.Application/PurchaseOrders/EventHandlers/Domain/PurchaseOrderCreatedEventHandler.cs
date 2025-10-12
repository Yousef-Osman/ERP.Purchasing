using ERP.Purchasing.Domain.PurchaseOrderAggregate.Events;
using ERP.SharedKernel.Interfaces;
using Microsoft.Extensions.Logging;

namespace ERP.Purchasing.Application.PurchaseOrders.EventHandlers.Domain;
public class PurchaseOrderCreatedEventHandler : IDomainEventHandler<PurchaseOrderCreatedEvent>
{
    private readonly ILogger<PurchaseOrderCreatedEventHandler> _logger;

    public PurchaseOrderCreatedEventHandler(ILogger<PurchaseOrderCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(PurchaseOrderCreatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "PurchaseOrder created: {PurchaseOrderNumber} at {OccurredOn}",
            domainEvent.PurchaseOrderNumber,
            domainEvent.OccurredOn);

        await Task.CompletedTask;
    }
}
