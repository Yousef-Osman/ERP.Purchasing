using ERP.Purchasing.Application.Common.DTOs;
using MediatR;

namespace ERP.Purchasing.Application.PurchaseOrders.Queries.GetRecentPurchaseOrders;
public record GetRecentPurchaseOrdersQuery(int Count = 7) : IRequest<List<PurchaseOrderDto>>;