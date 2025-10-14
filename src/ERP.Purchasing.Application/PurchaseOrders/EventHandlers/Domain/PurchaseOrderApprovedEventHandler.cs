using ERP.Purchasing.Application.Common.Interfaces;
using ERP.Purchasing.Domain.PurchaseOrderAggregate.Events;
using ERP.SharedKernel.Interfaces;
using ERP.SharedKernel.Messaging.Events;
using Microsoft.Extensions.Logging;

namespace ERP.Purchasing.Application.PurchaseOrders.EventHandlers.Domain;
public class PurchaseOrderApprovedEventHandler : IDomainEventHandler<PurchaseOrderApprovedEvent>
{
    private readonly IIntegrationEventPublisher _integrationEventPublisher;
    private readonly IPurchaseOrderRepository _repository;
    private readonly ILogger<PurchaseOrderApprovedEventHandler> _logger;

    public PurchaseOrderApprovedEventHandler(
        IIntegrationEventPublisher integrationEventPublisher,
        IPurchaseOrderRepository repository,
        ILogger<PurchaseOrderApprovedEventHandler> logger)
    {
        _integrationEventPublisher = integrationEventPublisher;
        _repository = repository;
        _logger = logger;
    }

    public async Task HandleAsync(PurchaseOrderApprovedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation(
                "Handling PurchaseOrderApprovedEvent for PO: {PONumber}",
                domainEvent.PurchaseOrderNumber);

            // Get full PO details
            var po = await _repository.GetByIdAsync(domainEvent.PurchaseOrderId);
            if (po == null)
            {
                _logger.LogWarning("PO {POId} not found", domainEvent.PurchaseOrderId);
                return;
            }

            // Create integration event
            var integrationEvent = new PurchaseOrderApprovedIntegrationEvent(
                po.Id,
                po.Number.Value,
                po.IssueDate,
                po.TotalPrice.Amount,
                po.TotalPrice.Currency,
                po.Items.Select(i => new PurchaseOrderItemDto
                {
                    GoodCode = i.GoodCode.Value,
                    Price = i.Price.Amount,
                    Currency = i.Price.Currency
                }).ToList()
            );

            // Publish to message broker
            await _integrationEventPublisher.PublishAsync(integrationEvent, cancellationToken);

            _logger.LogInformation(
                "Published PurchaseOrderApprovedIntegrationEvent for PO: {PONumber}",
                domainEvent.PurchaseOrderNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing integration event for PO: {PONumber}",
                domainEvent.PurchaseOrderNumber);
            // Don't throw - integration events should not break domain operations
        }
    }
}