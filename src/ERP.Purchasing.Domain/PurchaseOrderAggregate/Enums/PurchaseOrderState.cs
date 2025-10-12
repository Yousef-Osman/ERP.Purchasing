namespace ERP.Purchasing.Domain.PurchaseOrderAggregate.Enums;
public enum PurchaseOrderState
{
    Created,
    Approved,
    BeingShipped,
    Shipped,
    Closed,
    Deactivated
}