using ERP.Purchasing.Domain.PurchaseOrderAggregate.Enums;
using ERP.SharedKernel.Enums;
using ERP.SharedKernel.Pagination;

namespace ERP.Purchasing.Application.Common.Requests;
public class PurchaseOrderQueryRequest : PaginationParams
{
    public PurchaseOrderState? State { get; set; }
    public bool? IsActive { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? SearchTerm { get; set; }
    public string SortBy { get; set; } = "IssueDate";
    public SortDirection SortDirection { get; set; } = SortDirection.Descending;
}
