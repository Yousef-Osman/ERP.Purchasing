using ERP.Purchasing.Application.Common.DTOs;
using ERP.Purchasing.Application.Common.Requests;
using ERP.SharedKernel.Pagination;
using MediatR;

namespace ERP.Purchasing.Application.PurchaseOrders.Queries.GetAllPurchaseOrders;
public record GetAllPurchaseOrdersQuery : IRequest<PagedResult<PurchaseOrderDto>>
{
    public PurchaseOrderQueryRequest Request { get; }

    public GetAllPurchaseOrdersQuery(PurchaseOrderQueryRequest request)
    {
        Request = request ?? new PurchaseOrderQueryRequest();
    }
}
