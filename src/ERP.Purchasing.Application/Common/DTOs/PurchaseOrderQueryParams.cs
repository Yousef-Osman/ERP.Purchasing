using ERP.Purchasing.Domain.PurchaseOrderAggregate.Enums;

namespace ERP.Purchasing.Application.Common.DTOs;
public class PurchaseOrderQueryParams
{
    public PurchaseOrderState? State { get; set; }
    public bool? IsActive { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? SearchTerm { get; set; }
    public string SortBy { get; set; } = "IssueDate";
    public bool SortDescending { get; set; } = true;
    public int Skip { get; set; } = 0;
    public int Take { get; set; } = 10;
}
