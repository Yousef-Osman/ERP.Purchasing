using ERP.Purchasing.Domain.PurchaseOrderAggregate.Services;
using ERP.SharedKernel.ValueObjects.DocumentNumber;

namespace ERP.Purchasing.Domain.PurchaseOrderAggregate.Factories;
public class PurchaseOrderNumberGeneratorFactory: IPurchaseOrderNumberGeneratorFactory
{
    private readonly Dictionary<string, Func<IDocumentNumberGenerator>> _strategies;

    public PurchaseOrderNumberGeneratorFactory()
    {
        _strategies = new Dictionary<string, Func<IDocumentNumberGenerator>>
            {
                { "sequential", () => new PurchaseOrderNumberGenerator() },
                { "timestamp", () => new TimestampPurchaseOrderNumberGenerator() }
            };
    }

    public IDocumentNumberGenerator CreatePurchaseOrderNumberGenerator(string strategy = "sequential")
    {
        if (!_strategies.ContainsKey(strategy.ToLower()))
            throw new ArgumentException($"Unknown strategy: {strategy}", nameof(strategy));

        return _strategies[strategy.ToLower()]();
    }

    public void RegisterStrategy(string name, Func<IDocumentNumberGenerator> factory)
    {
        _strategies[name.ToLower()] = factory;
    }
}
