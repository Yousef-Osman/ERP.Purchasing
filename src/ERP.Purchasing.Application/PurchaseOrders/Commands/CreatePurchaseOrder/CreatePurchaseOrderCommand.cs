using ERP.Purchasing.Application.Common.DTOs;
using MediatR;

namespace ERP.Purchasing.Application.PurchaseOrders.Commands.CreatePurchaseOrder;
public record CreatePurchaseOrderCommand : IRequest<PurchaseOrderDto>
{
    public DateTime IssueDate { get; set; }
    public List<CreatePurchaseOrderItemDto> Items { get; set; }
    public string NumberGenerationStrategy { get; set; } = "sequential";
}
