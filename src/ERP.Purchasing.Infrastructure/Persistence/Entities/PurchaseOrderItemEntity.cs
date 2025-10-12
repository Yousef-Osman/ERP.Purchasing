namespace ERP.Purchasing.Infrastructure.Persistence.Entities;
public class PurchaseOrderItemEntity
{
    public Guid Id { get; set; }
    public int SerialNumber { get; set; }
    public string GoodCode { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; }
    public Guid PurchaseOrderId { get; set; }
    public virtual PurchaseOrderEntity PurchaseOrder { get; set; }
}
