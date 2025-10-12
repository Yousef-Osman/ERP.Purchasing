using ERP.Purchasing.Domain.PurchaseOrderAggregate.Events;
using ERP.SharedKernel.Interfaces;
using Microsoft.Extensions.Logging;

namespace ERP.Purchasing.Application.PurchaseOrders.EventHandlers.Domain;
public class PurchaseOrderApprovedEventHandler : IDomainEventHandler<PurchaseOrderApprovedEvent>
{
    private readonly ILogger<PurchaseOrderApprovedEventHandler> _logger;

    public PurchaseOrderApprovedEventHandler(ILogger<PurchaseOrderApprovedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(PurchaseOrderApprovedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "PurchaseOrder approved: {PurchaseOrderNumber} at {OccurredOn}",
            domainEvent.PurchaseOrderNumber,
            domainEvent.OccurredOn);

        await Task.CompletedTask;
    }
}