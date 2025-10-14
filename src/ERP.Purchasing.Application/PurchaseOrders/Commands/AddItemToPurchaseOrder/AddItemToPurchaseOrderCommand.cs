using ERP.Purchasing.Application.Common.DTOs;
using MediatR;

namespace ERP.Purchasing.Application.PurchaseOrders.Commands.AddItemToPurchaseOrder;
public record AddItemToPurchaseOrderCommand : IRequest<PurchaseOrderDto>
{
    public Guid PurchaseOrderId { get; set; }
    public string GoodCode { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; } = "USD";
}
