using ERP.Purchasing.Application.Common.DTOs;
using MediatR;

namespace ERP.Purchasing.Application.PurchaseOrders.Commands.DeactivatePurchaseOrder;
public record DeactivatePurchaseOrderCommand(Guid PurchaseOrderId)
    : IRequest<PurchaseOrderDto>;