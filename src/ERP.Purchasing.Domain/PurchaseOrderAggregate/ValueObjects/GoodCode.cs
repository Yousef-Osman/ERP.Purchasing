using ERP.SharedKernel.Abstractions;

namespace ERP.Purchasing.Domain.PurchaseOrderAggregate.ValueObjects;
public class GoodCode : ValueObject
{
    public string Value { get; }

    public GoodCode(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Good code cannot be empty", nameof(value));

        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
