using ERP.SharedKernel.ValueObjects.DocumentNumber;

namespace ERP.Purchasing.Domain.PurchaseOrderAggregate.ValueObjects;
public class PurchaseOrderNumber : DocumentNumber
{
    public PurchaseOrderNumber(string value) : base(value)
    {
        if (!value.StartsWith("PO"))
            throw new ArgumentException("PO number must start with 'PO'", nameof(value));
    }

    public static PurchaseOrderNumber Create(IDocumentNumberGenerator generator)
    {
        return new PurchaseOrderNumber(generator.Generate("PO"));
    }
}
