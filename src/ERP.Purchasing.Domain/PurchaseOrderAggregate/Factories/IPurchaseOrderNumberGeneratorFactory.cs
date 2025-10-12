
using ERP.SharedKernel.ValueObjects.DocumentNumber;

namespace ERP.Purchasing.Domain.PurchaseOrderAggregate.Factories;
public interface IPurchaseOrderNumberGeneratorFactory
{
    IDocumentNumberGenerator CreatePurchaseOrderNumberGenerator(string strategy = "sequential");
}
