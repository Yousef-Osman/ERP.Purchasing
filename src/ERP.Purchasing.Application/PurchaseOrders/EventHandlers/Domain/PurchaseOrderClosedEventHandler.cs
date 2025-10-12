using ERP.Purchasing.Domain.PurchaseOrderAggregate.Events;
using ERP.SharedKernel.Interfaces;
using Microsoft.Extensions.Logging;

namespace ERP.Purchasing.Application.PurchaseOrders.EventHandlers.Domain;
public class PurchaseOrderClosedEventHandler : IDomainEventHandler<PurchaseOrderClosedEvent>
{
    private readonly ILogger<PurchaseOrderClosedEventHandler> _logger;

    public PurchaseOrderClosedEventHandler(ILogger<PurchaseOrderClosedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(PurchaseOrderClosedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "PurchaseOrder closed: {PurchaseOrderNumber} at {OccurredOn}",
            domainEvent.PurchaseOrderNumber,
            domainEvent.OccurredOn);

        await Task.CompletedTask;
    }
}
