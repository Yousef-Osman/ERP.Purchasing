using ERP.Purchasing.Domain.PurchaseOrderAggregate.Factories;
using ERP.Purchasing.Domain.PurchaseOrderAggregate.ValueObjects;

namespace ERP.Purchasing.Domain.PurchaseOrderAggregate.Services;
public class PurchaseOrderDomainService
{
    private readonly IPurchaseOrderNumberGeneratorFactory _numberGeneratorFactory;

    public PurchaseOrderDomainService(IPurchaseOrderNumberGeneratorFactory numberGeneratorFactory)
    {
        _numberGeneratorFactory = numberGeneratorFactory ??
            throw new ArgumentNullException(nameof(numberGeneratorFactory));
    }

    public PurchaseOrder CreatePurchaseOrder(
        DateTime issueDate,
        string numberGenerationStrategy = "sequential")
    {
        var generator = _numberGeneratorFactory.CreatePurchaseOrderNumberGenerator(numberGenerationStrategy);
        var number = PurchaseOrderNumber.Create(generator);
        return new PurchaseOrder(number, issueDate);
    }

    //public void ValidatePurchaseOrderForApproval(PurchaseOrder purchaseOrder)
    //{
    //    if (purchaseOrder == null)
    //        throw new ArgumentNullException(nameof(purchaseOrder));

    //    if (!purchaseOrder.IsActive)
    //        throw new DomainException("Cannot approve an inactive purchase order");

    //    if (!purchaseOrder.Items.Any())
    //        throw new DomainException("Cannot approve purchase order without items");

    //    if (purchaseOrder.State != PurchaseOrderState.Created)
    //        throw new DomainException($"Purchase order must be in Created state to be approved. Current state: {purchaseOrder.State}");
    //}

    //public void ValidatePurchaseOrderItems(
    //    IEnumerable<(GoodCode GoodCode, Money Price)> items)
    //{
    //    if (items == null || !items.Any())
    //        throw new DomainException("Purchase order must have at least one item");

    //    var goodCodes = items.Select(i => i.GoodCode).ToList();
    //    var duplicates = goodCodes.GroupBy(g => g)
    //        .Where(g => g.Count() > 1)
    //        .Select(g => g.Key)
    //        .ToList();

    //    if (duplicates.Any())
    //        throw new DomainException($"Duplicate goods found: {string.Join(", ", duplicates.Select(d => d.Value))}");

    //    foreach (var item in items)
    //    {
    //        if (item.Price == null || item.Price.Amount <= 0)
    //            throw new DomainException($"Invalid price for good {item.GoodCode.Value}");
    //    }
    //}
}
