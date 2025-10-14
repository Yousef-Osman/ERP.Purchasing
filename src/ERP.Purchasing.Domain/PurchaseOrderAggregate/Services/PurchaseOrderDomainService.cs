using ERP.Purchasing.Domain.PurchaseOrderAggregate.ValueObjects;
using ERP.SharedKernel.Enums;
using ERP.SharedKernel.Factories;

namespace ERP.Purchasing.Domain.PurchaseOrderAggregate.Services;
public class PurchaseOrderDomainService
{
    private readonly IDocumentNumberGeneratorFactory _numberGeneratorFactory;

    public PurchaseOrderDomainService(IDocumentNumberGeneratorFactory numberGeneratorFactory)
    {
        _numberGeneratorFactory = numberGeneratorFactory ??
            throw new ArgumentNullException(nameof(numberGeneratorFactory));
    }

    public PurchaseOrder CreatePurchaseOrder(DateTime issueDate,
        DocumentNumberGenerationMethod numberGenerationStrategy = DocumentNumberGenerationMethod.Timestamp)
    {
        var generator = _numberGeneratorFactory.CreateDocumentNumberGenerator(numberGenerationStrategy);
        var number = PurchaseOrderNumber.Create(generator);

        return new PurchaseOrder(number, issueDate);
    }
}
