using ERP.Purchasing.Application.Common.DTOs;
using MediatR;

namespace ERP.Purchasing.Application.PurchaseOrders.Commands.CreateMultiplePurchaseOrders;
public record CreateMultiplePurchaseOrdersCommand(List<CreatePurchaseOrderRequest> PurchaseOrders)
    : IRequest<List<PurchaseOrderDto>>;