using ERP.Purchasing.Application.Common.DTOs;
using ERP.Purchasing.Domain.PurchaseOrderAggregate;
using ERP.Purchasing.Domain.PurchaseOrderAggregate.Entities;

namespace ERP.Purchasing.Application.Common.Mappers;
public static class PurchaseOrderMapper
{
    public static PurchaseOrderDto ToDto(PurchaseOrder po)
    {
        if (po == null) return null;

        return new PurchaseOrderDto
        {
            Id = po.Id,
            Number = po.Number.Value,
            IssueDate = po.IssueDate,
            TotalPrice = po.TotalPrice.Amount,
            Currency = po.TotalPrice.Currency,
            State = po.State.ToString(),
            IsActive = po.IsActive,
            Items = po.Items.Select(ToItemDto).ToList()
        };
    }

    public static PurchaseOrderItemDto ToItemDto(PurchaseOrderItem item)
    {
        if (item == null) return null;

        return new PurchaseOrderItemDto
        {
            Id = item.Id,
            SerialNumber = item.SerialNumber,
            GoodCode = item.GoodCode.Value,
            Price = item.Price.Amount,
            Currency = item.Price.Currency
        };
    }
}
