using ERP.Purchasing.Application.Common.DTOs;
using MediatR;

namespace ERP.Purchasing.Application.PurchaseOrders.Commands.ApprovePurchaseOrder;
public record ApprovePurchaseOrderCommand(Guid PurchaseOrderId) : IRequest<PurchaseOrderDto>;