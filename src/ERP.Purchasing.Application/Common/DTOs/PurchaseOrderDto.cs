using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Purchasing.Application.Common.DTOs;
public class PurchaseOrderDto
{
    public Guid Id { get; set; }
    public string Number { get; set; }
    public DateTime IssueDate { get; set; }
    public decimal TotalPrice { get; set; }
    public string Currency { get; set; }
    public string State { get; set; }
    public bool IsActive { get; set; }
    public List<PurchaseOrderItemDto> Items { get; set; }
}
