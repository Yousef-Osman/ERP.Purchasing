using ERP.Purchasing.Application.Common.Interfaces;
using ERP.Purchasing.Domain.PurchaseOrderAggregate.Enums;
using ERP.Purchasing.Domain.PurchaseOrderAggregate.ValueObjects;
using ERP.SharedKernel.Messaging.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace ERP.Purchasing.Application.PurchaseOrders.EventHandlers.Integration;
public class ShippingOrderClosedConsumer : IConsumer<ShippingOrderClosedIntegrationEvent>
{
    private readonly IPurchaseOrderRepository _repository;
    private readonly ILogger<ShippingOrderClosedConsumer> _logger;

    public ShippingOrderClosedConsumer(
        IPurchaseOrderRepository repository,
        ILogger<ShippingOrderClosedConsumer> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ShippingOrderClosedIntegrationEvent> context)
    {
        var message = context.Message;

        _logger.LogInformation(
            "Received ShippingOrderClosedIntegrationEvent for PO: {PONumber}",
            message.ReferencedPurchaseOrderNumber);

        try
        {
            // Get PO and update state
            var poNumber = new PurchaseOrderNumber(message.ReferencedPurchaseOrderNumber);
            var po = await _repository.GetByNumberAsync(poNumber);

            if (po == null)
            {
                _logger.LogWarning("PO {PONumber} not found", message.ReferencedPurchaseOrderNumber);
                return;
            }

            if (po.State == PurchaseOrderState.BeingShipped)
            {
                po.Close();
                await _repository.UpdateAsync(po);
                await _repository.SaveChangesAsync();

                _logger.LogInformation(
                    "Updated PO {PONumber} to Closed state",
                    message.ReferencedPurchaseOrderNumber);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing ShippingOrderClosedIntegrationEvent for PO: {PONumber}",
                message.ReferencedPurchaseOrderNumber);
            throw; // Will trigger retry
        }
    }
}
