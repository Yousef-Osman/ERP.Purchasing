using ERP.Purchasing.Domain.PurchaseOrderAggregate.ValueObjects;
using ERP.SharedKernel.Abstractions;

namespace ERP.Purchasing.Domain.PurchaseOrderAggregate.Entities;
public class PurchaseOrderItem : Entity<Guid>
{
    public int SerialNumber { get; private set; }
    public GoodCode GoodCode { get; private set; }
    public Money Price { get; private set; }
    public Guid PurchaseOrderId { get; private set; }

    private PurchaseOrder _purchaseOrder;
    public virtual PurchaseOrder PurchaseOrder
    {
        get => _purchaseOrder;
        private set => _purchaseOrder = value;
    }

    private PurchaseOrderItem() : base() { }

    internal PurchaseOrderItem(int serialNumber, GoodCode goodCode, Money price, Guid purchaseOrderId)
        : base(Guid.NewGuid())
    {
        if (serialNumber <= 0)
            throw new ArgumentException("Serial number must be positive", nameof(serialNumber));

        SerialNumber = serialNumber;
        GoodCode = goodCode ?? throw new ArgumentNullException(nameof(goodCode));
        Price = price ?? throw new ArgumentNullException(nameof(price));
        PurchaseOrderId = purchaseOrderId;
    }

    internal void UpdatePrice(Money newPrice)
    {
        Price = newPrice ?? throw new ArgumentNullException(nameof(newPrice));
    }
}
