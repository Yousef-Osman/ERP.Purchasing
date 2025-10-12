using ERP.SharedKernel.Abstractions;

namespace ERP.Purchasing.Domain.PurchaseOrderAggregate.Events;
public class PurchaseOrderApprovedEvent : DomainEvent
{
    public Guid PurchaseOrderId { get; }
    public string PurchaseOrderNumber { get; }

    public PurchaseOrderApprovedEvent(Guid purchaseOrderId, string purchaseOrderNumber)
    {
        PurchaseOrderId = purchaseOrderId;
        PurchaseOrderNumber = purchaseOrderNumber;
    }
}