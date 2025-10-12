namespace ERP.Purchasing.Infrastructure.Persistence.Entities;
public class PurchaseOrderEntity
{
    public Guid Id { get; set; }
    public string Number { get; set; }
    public DateTime IssueDate { get; set; }
    public decimal TotalPrice { get; set; }
    public string Currency { get; set; }
    public int State { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public byte[] RowVersion { get; set; }

    public virtual ICollection<PurchaseOrderItemEntity> Items { get; set; }

    public PurchaseOrderEntity()
    {
        Items = new HashSet<PurchaseOrderItemEntity>();
    }
}
