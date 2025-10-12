using ERP.Purchasing.Application.Common.DTOs;
using MediatR;

namespace ERP.Purchasing.Application.PurchaseOrders.Commands.ApprovePurchaseOrder;
public record ApprovePurchaseOrderCommand : IRequest<PurchaseOrderDto>
{
    public Guid PurchaseOrderId { get; set; }
}
