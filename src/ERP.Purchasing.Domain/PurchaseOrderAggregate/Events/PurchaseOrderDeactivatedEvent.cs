using ERP.SharedKernel.Abstractions;

namespace ERP.Purchasing.Domain.PurchaseOrderAggregate.Events;
public class PurchaseOrderDeactivatedEvent : DomainEvent
{
    public Guid PurchaseOrderId { get; }
    public string PurchaseOrderNumber { get; }

    public PurchaseOrderDeactivatedEvent(Guid purchaseOrderId, string purchaseOrderNumber)
    {
        PurchaseOrderId = purchaseOrderId;
        PurchaseOrderNumber = purchaseOrderNumber;
    }
}
