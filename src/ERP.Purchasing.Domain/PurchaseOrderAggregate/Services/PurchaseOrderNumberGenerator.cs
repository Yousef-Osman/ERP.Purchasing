using ERP.SharedKernel.ValueObjects.DocumentNumber;

namespace ERP.Purchasing.Domain.PurchaseOrderAggregate.Services;
public class PurchaseOrderNumberGenerator : IDocumentNumberGenerator
{
    private static int _counter = 1000;
    private static readonly object _lock = new object();

    public virtual string Generate()
    {
        lock (_lock)
        {
            return $"PO{_counter++:D6}";
        }
    }
}
