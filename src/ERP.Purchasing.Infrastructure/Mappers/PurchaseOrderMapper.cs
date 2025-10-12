using System.Reflection;
using ERP.Purchasing.Domain.PurchaseOrderAggregate;
using ERP.Purchasing.Domain.PurchaseOrderAggregate.Entities;
using ERP.Purchasing.Domain.PurchaseOrderAggregate.Enums;
using ERP.Purchasing.Domain.PurchaseOrderAggregate.ValueObjects;
using ERP.Purchasing.Infrastructure.Persistence.Entities;

namespace ERP.Purchasing.Infrastructure.Mappers;
public static class PurchaseOrderMapper
{
    public static PurchaseOrder ToDomain(PurchaseOrderEntity entity)
    {
        if (entity == null) return null;

        var number = new PurchaseOrderNumber(entity.Number);
        var po = (PurchaseOrder)Activator.CreateInstance(
            typeof(PurchaseOrder),
            BindingFlags.NonPublic | BindingFlags.Instance,
            null, new object[] { }, null);

        SetProperty(po, "Id", entity.Id);
        SetProperty(po, "Number", number);
        SetProperty(po, "IssueDate", entity.IssueDate);
        SetProperty(po, "TotalPrice", new Money(entity.TotalPrice, entity.Currency));
        SetProperty(po, "State", (PurchaseOrderState)entity.State);
        SetProperty(po, "IsActive", entity.IsActive);

        if (entity.Items != null && entity.Items.Any())
        {
            var itemsList = GetPrivateField<List<PurchaseOrderItem>>(po, "_items");
            itemsList.Clear();

            foreach (var itemEntity in entity.Items.OrderBy(i => i.SerialNumber))
            {
                var item = PurchaseOrderItemMapper.ToDomain(itemEntity);
                itemsList.Add(item);
            }
        }

        return po;
    }

    public static PurchaseOrderEntity ToEntity(PurchaseOrder domain)
    {
        if (domain == null) return null;

        var entity = new PurchaseOrderEntity
        {
            Id = domain.Id,
            Number = domain.Number.Value,
            IssueDate = domain.IssueDate,
            TotalPrice = domain.TotalPrice.Amount,
            Currency = domain.TotalPrice.Currency,
            State = (int)domain.State,
            IsActive = domain.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var item in domain.Items)
        {
            entity.Items.Add(PurchaseOrderItemMapper.ToEntity(item));
        }

        return entity;
    }

    public static void UpdateEntity(PurchaseOrderEntity entity, PurchaseOrder domain)
    {
        entity.Number = domain.Number.Value;
        entity.IssueDate = domain.IssueDate;
        entity.TotalPrice = domain.TotalPrice.Amount;
        entity.Currency = domain.TotalPrice.Currency;
        entity.State = (int)domain.State;
        entity.IsActive = domain.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;

        var existingItems = entity.Items.ToList();
        var domainItems = domain.Items.ToList();

        foreach (var existingItem in existingItems)
        {
            if (!domainItems.Any(di => di.Id == existingItem.Id))
                entity.Items.Remove(existingItem);
        }

        foreach (var domainItem in domainItems)
        {
            var existingItem = existingItems.FirstOrDefault(ei => ei.Id == domainItem.Id);
            if (existingItem == null)
                entity.Items.Add(PurchaseOrderItemMapper.ToEntity(domainItem));
            else
                PurchaseOrderItemMapper.UpdateEntity(existingItem, domainItem);
        }
    }

    private static void SetProperty<T>(T obj, string propertyName, object value)
    {
        var property = typeof(T).GetProperty(propertyName,
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        property?.SetValue(obj, value);
    }

    private static TField GetPrivateField<TField>(object obj, string fieldName)
    {
        var field = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        return (TField)field?.GetValue(obj);
    }
}