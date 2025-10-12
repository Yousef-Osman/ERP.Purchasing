using ERP.Purchasing.Application.Common.DTOs;
using MediatR;

namespace ERP.Purchasing.Application.PurchaseOrders.Commands.UpdateItemPrice;
public record UpdateItemPriceCommand : IRequest<PurchaseOrderDto>
{
    public Guid PurchaseOrderId { get; set; }
    public string GoodCode { get; set; }
    public decimal NewPrice { get; set; }
    public string Currency { get; set; } = "USD";
}
