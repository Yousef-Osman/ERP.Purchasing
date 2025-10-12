using ERP.SharedKernel.Abstractions;

namespace ERP.Purchasing.Domain.PurchaseOrderAggregate.Events;
public class PurchaseOrderBeingShippedEvent : DomainEvent
{
    public Guid PurchaseOrderId { get; }
    public string PurchaseOrderNumber { get; }

    public PurchaseOrderBeingShippedEvent(Guid purchaseOrderId, string purchaseOrderNumber)
    {
        PurchaseOrderId = purchaseOrderId;
        PurchaseOrderNumber = purchaseOrderNumber;
    }
}