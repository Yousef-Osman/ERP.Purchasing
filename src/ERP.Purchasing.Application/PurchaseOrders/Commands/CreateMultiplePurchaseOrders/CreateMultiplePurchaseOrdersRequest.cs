using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.Purchasing.Application.Common.DTOs;

namespace ERP.Purchasing.Application.PurchaseOrders.Commands.CreateMultiplePurchaseOrders;
public class CreatePurchaseOrderRequest
{
    public DateTime IssueDate { get; set; }
    public List<CreatePurchaseOrderItemDto> Items { get; set; }
}
