using ERP.Purchasing.Application.Common.DTOs;
using MediatR;

namespace ERP.Purchasing.Application.PurchaseOrders.Queries.GetPurchaseOrderByNumber;
public record GetPurchaseOrderByNumberQuery : IRequest<PurchaseOrderDto>
{
    public string Number { get; set; }
}
