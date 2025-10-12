using ERP.Purchasing.Domain.PurchaseOrderAggregate.Events;
using ERP.SharedKernel.Interfaces;
using Microsoft.Extensions.Logging;

namespace ERP.Purchasing.Application.PurchaseOrders.EventHandlers.Domain;
public class PurchaseOrderBeingShippedEventHandler : IDomainEventHandler<PurchaseOrderBeingShippedEvent>
{
    private readonly ILogger<PurchaseOrderBeingShippedEventHandler> _logger;

    public PurchaseOrderBeingShippedEventHandler(ILogger<PurchaseOrderBeingShippedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(PurchaseOrderBeingShippedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "PurchaseOrder being shipped: {PurchaseOrderNumber} at {OccurredOn}",
            domainEvent.PurchaseOrderNumber,
            domainEvent.OccurredOn);

        await Task.CompletedTask;
    }
}
