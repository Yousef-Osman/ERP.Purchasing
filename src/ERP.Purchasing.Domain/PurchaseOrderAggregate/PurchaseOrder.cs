using ERP.Purchasing.Domain.PurchaseOrderAggregate.Entities;
using ERP.Purchasing.Domain.PurchaseOrderAggregate.Enums;
using ERP.Purchasing.Domain.PurchaseOrderAggregate.Events;
using ERP.Purchasing.Domain.PurchaseOrderAggregate.ValueObjects;
using ERP.SharedKernel.Abstractions;

namespace ERP.Purchasing.Domain.PurchaseOrderAggregate;
public class PurchaseOrder : AggregateRoot<Guid>
{
    public PurchaseOrderNumber Number { get; private set; }
    public DateTime IssueDate { get; private set; }
    public Money TotalPrice { get; private set; }
    public PurchaseOrderState State { get; private set; }
    public bool IsActive { get; private set; }

    private readonly List<PurchaseOrderItem> _items = new();
    public virtual IReadOnlyCollection<PurchaseOrderItem> Items => _items.AsReadOnly();

    private PurchaseOrder() : base() { }

    public PurchaseOrder(PurchaseOrderNumber number, DateTime issueDate)
        : base(Guid.NewGuid())
    {
        Number = number ?? throw new ArgumentNullException(nameof(number));
        IssueDate = issueDate;
        TotalPrice = new Money(0);
        State = PurchaseOrderState.Created;
        IsActive = true;

        AddDomainEvent(new PurchaseOrderCreatedEvent(Id, Number.Value));
    }

    public void AddItem(GoodCode goodCode, Money price)
    {
        if (goodCode == null) throw new ArgumentNullException(nameof(goodCode));
        if (price == null) throw new ArgumentNullException(nameof(price));

        if (!IsActive)
            throw new InvalidOperationException("Cannot add items to an inactive PO");

        if (_items.Any(i => i.GoodCode.Equals(goodCode)))
            throw new InvalidOperationException($"Good {goodCode} already exists in this PO");

        var serialNumber = _items.Count + 1;
        var item = new PurchaseOrderItem(serialNumber, goodCode, price, Id);
        _items.Add(item);

        RecalculateTotalPrice();
    }

    public void RemoveItem(GoodCode goodCode)
    {
        if (!IsActive)
            throw new InvalidOperationException("Cannot remove items from an inactive PO");

        var item = _items.FirstOrDefault(i => i.GoodCode.Equals(goodCode));

        if (item == null)
            throw new InvalidOperationException($"Good {goodCode} not found in this PO");

        _items.Remove(item);
        ReorderItems();
        RecalculateTotalPrice();
    }

    public void UpdateItemPrice(GoodCode goodCode, Money newPrice)
    {
        if (!IsActive)
            throw new InvalidOperationException("Cannot update items form an inactive PO");

        var item = _items.FirstOrDefault(i => i.GoodCode.Equals(goodCode));
        if (item == null)
            throw new InvalidOperationException($"Good {goodCode} not found in this PO");

        item.UpdatePrice(newPrice);
        RecalculateTotalPrice();
    }

    private void ReorderItems()
    {
        for (int i = 0; i < _items.Count; i++)
        {
            typeof(PurchaseOrderItem)
                .GetProperty(nameof(PurchaseOrderItem.SerialNumber))
                .SetValue(_items[i], i + 1);
        }
    }

    private void RecalculateTotalPrice()
    {
        if (_items.Count == 0)
        {
            TotalPrice = new Money(0);
        }
        else
        {
            TotalPrice = _items
                .Select(i => i.Price)
                .Aggregate((a, b) => a + b);
        }
    }

    public void Approve()
    {
        if (State != PurchaseOrderState.Created)
            throw new InvalidOperationException($"Cannot approve PO in {State} state");

        if (!IsActive)
            throw new InvalidOperationException("Cannot approve an inactive PO");

        if (_items.Count == 0)
            throw new InvalidOperationException("Cannot approve PO without items");

        State = PurchaseOrderState.Approved;
        AddDomainEvent(new PurchaseOrderApprovedEvent(Id, Number.Value));
    }

    public void MarkAsBeingShipped()
    {
        if (State != PurchaseOrderState.Approved)
            throw new InvalidOperationException($"Cannot mark PO as being shipped from {State} state");

        State = PurchaseOrderState.BeingShipped;
        AddDomainEvent(new PurchaseOrderBeingShippedEvent(Id, Number.Value));
    }

    public void Close()
    {
        if (State != PurchaseOrderState.BeingShipped)
            throw new InvalidOperationException($"Cannot close PO from {State} state");

        State = PurchaseOrderState.Closed;
        AddDomainEvent(new PurchaseOrderClosedEvent(Id, Number.Value));
    }

    public void Deactivate()
    {
        if (!IsActive)
            throw new InvalidOperationException("PO is already deactivated");

        IsActive = false;
        AddDomainEvent(new PurchaseOrderDeactivatedEvent(Id, Number.Value));
    }

    public void Activate()
    {
        if (IsActive)
            throw new InvalidOperationException("PO is already active");

        IsActive = true;
        AddDomainEvent(new PurchaseOrderActivatedEvent(Id, Number.Value));
    }
}
