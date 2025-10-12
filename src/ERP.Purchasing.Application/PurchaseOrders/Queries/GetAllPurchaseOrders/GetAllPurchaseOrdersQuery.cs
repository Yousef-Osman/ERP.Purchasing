using ERP.Purchasing.Application.Common.DTOs;
using ERP.Purchasing.Domain.PurchaseOrderAggregate.Enums;
using MediatR;

namespace ERP.Purchasing.Application.PurchaseOrders.Queries.GetAllPurchaseOrders;
public record GetAllPurchaseOrdersQuery : IRequest<List<PurchaseOrderDto>>
{
    public PurchaseOrderState? State { get; set; }
    public bool? IsActive { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
}
