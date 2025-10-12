using ERP.Purchasing.Application.Common.DTOs;
using MediatR;

namespace ERP.Purchasing.Application.PurchaseOrders.Commands.DeactivatePurchaseOrder;
public record DeactivatePurchaseOrderCommand : IRequest<PurchaseOrderDto>
{
    public Guid PurchaseOrderId { get; set; }
}
