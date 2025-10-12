using ERP.SharedKernel.Abstractions;

namespace ERP.Purchasing.Domain.PurchaseOrderAggregate.Events;
public class PurchaseOrderActivatedEvent : DomainEvent
{
    public Guid PurchaseOrderId { get; }
    public string PurchaseOrderNumber { get; }

    public PurchaseOrderActivatedEvent(Guid purchaseOrderId, string purchaseOrderNumber)
    {
        PurchaseOrderId = purchaseOrderId;
        PurchaseOrderNumber = purchaseOrderNumber;
    }
}
