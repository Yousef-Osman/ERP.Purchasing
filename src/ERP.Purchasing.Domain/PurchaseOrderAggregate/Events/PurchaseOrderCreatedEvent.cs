using ERP.SharedKernel.Abstractions;

namespace ERP.Purchasing.Domain.PurchaseOrderAggregate.Events;
public class PurchaseOrderCreatedEvent : DomainEvent
{
    public Guid PurchaseOrderId { get; }
    public string PurchaseOrderNumber { get; }

    public PurchaseOrderCreatedEvent(Guid purchaseOrderId, string purchaseOrderNumber)
    {
        PurchaseOrderId = purchaseOrderId;
        PurchaseOrderNumber = purchaseOrderNumber;
    }
}
