using ERP.SharedKernel.ValueObjects.DocumentNumber;

namespace ERP.Purchasing.Domain.PurchaseOrderAggregate.Services;
public class TimestampPurchaseOrderNumberGenerator : IDocumentNumberGenerator
{
    public string Generate()
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
        return $"PO{timestamp}";
    }
}
