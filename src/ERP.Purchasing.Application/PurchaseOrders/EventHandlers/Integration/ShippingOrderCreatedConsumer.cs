using ERP.Purchasing.Application.Common.Interfaces;
using ERP.Purchasing.Domain.PurchaseOrderAggregate.Enums;
using ERP.Purchasing.Domain.PurchaseOrderAggregate.ValueObjects;
using ERP.SharedKernel.Messaging.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace ERP.Purchasing.Application.PurchaseOrders.EventHandlers.Integration;
public class ShippingOrderCreatedConsumer : IConsumer<ShippingOrderCreatedIntegrationEvent>
{
    private readonly IPurchaseOrderRepository _repository;
    private readonly ILogger<ShippingOrderCreatedConsumer> _logger;

    public ShippingOrderCreatedConsumer(
        IPurchaseOrderRepository repository,
        ILogger<ShippingOrderCreatedConsumer> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ShippingOrderCreatedIntegrationEvent> context)
    {
        var message = context.Message;

        _logger.LogInformation(
            "Received ShippingOrderCreatedIntegrationEvent for PO: {PONumber}",
            message.PurchaseOrderNumber);

        try
        {
            // Get PO and update state
            var poNumber = new PurchaseOrderNumber(message.PurchaseOrderNumber);
            var po = await _repository.GetByNumberAsync(poNumber);

            if (po == null)
            {
                _logger.LogWarning("PO {PONumber} not found", message.PurchaseOrderNumber);
                return;
            }

            if (po.State == PurchaseOrderState.Approved)
            {
                po.MarkAsBeingShipped();
                await _repository.UpdateAsync(po);
                await _repository.SaveChangesAsync();

                _logger.LogInformation(
                    "Updated PO {PONumber} to BeingShipped state",
                    message.PurchaseOrderNumber);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing ShippingOrderCreatedIntegrationEvent for PO: {PONumber}",
                message.PurchaseOrderNumber);
            throw; // Will trigger retry
        }
    }
}
