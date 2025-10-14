using ERP.Purchasing.Application.Common.DTOs;
using ERP.SharedKernel.Enums;
using MediatR;

namespace ERP.Purchasing.Application.PurchaseOrders.Commands.CreatePurchaseOrder;
public record CreatePurchaseOrderCommand : IRequest<PurchaseOrderDto>
{
    public DateTime IssueDate { get; }
    public List<CreatePurchaseOrderItemDto> Items { get; }
    public DocumentNumberGenerationMethod NumberGenerationStrategy { get; } = DocumentNumberGenerationMethod.Timestamp;
}
