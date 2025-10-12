using System.Reflection;
using ERP.Purchasing.Domain.PurchaseOrderAggregate.Entities;
using ERP.Purchasing.Domain.PurchaseOrderAggregate.ValueObjects;
using ERP.Purchasing.Infrastructure.Persistence.Entities;
using ERP.SharedKernel.Abstractions;

namespace ERP.Purchasing.Infrastructure.Mappers;
public static class PurchaseOrderItemMapper
{
    public static PurchaseOrderItem ToDomain(PurchaseOrderItemEntity entity)
    {
        if (entity == null) return null;

        var item = (PurchaseOrderItem)Activator.CreateInstance(
            typeof(PurchaseOrderItem),
            BindingFlags.NonPublic | BindingFlags.Instance,
            null, new object[] { }, null);

        SetProperty(item, "Id", entity.Id);
        SetProperty(item, "SerialNumber", entity.SerialNumber);
        SetProperty(item, "GoodCode", new GoodCode(entity.GoodCode));
        SetProperty(item, "Price", new Money(entity.Price, entity.Currency));
        SetProperty(item, "PurchaseOrderId", entity.PurchaseOrderId);

        return item;
    }

    public static PurchaseOrderItemEntity ToEntity(PurchaseOrderItem domain)
    {
        if (domain == null) return null;

        return new PurchaseOrderItemEntity
        {
            Id = domain.Id,
            SerialNumber = domain.SerialNumber,
            GoodCode = domain.GoodCode.Value,
            Price = domain.Price.Amount,
            Currency = domain.Price.Currency,
            PurchaseOrderId = domain.PurchaseOrderId
        };
    }

    public static void UpdateEntity(PurchaseOrderItemEntity entity, PurchaseOrderItem domain)
    {
        entity.SerialNumber = domain.SerialNumber;
        entity.GoodCode = domain.GoodCode.Value;
        entity.Price = domain.Price.Amount;
        entity.Currency = domain.Price.Currency;
    }

    private static void SetProperty<T>(T obj, string propertyName, object value)
    {
        var property = typeof(T).GetProperty(propertyName,
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        property?.SetValue(obj, value);
    }
}
