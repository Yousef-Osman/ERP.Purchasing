using ERP.Purchasing.Application.Common.DTOs;
using MediatR;

namespace ERP.Purchasing.Application.PurchaseOrders.Queries.GetPurchaseOrderById;
public record GetPurchaseOrderByIdQuery(Guid Id) : IRequest<PurchaseOrderDto>;
