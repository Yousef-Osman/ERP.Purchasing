using FluentValidation;

namespace ERP.Purchasing.Application.PurchaseOrders.Commands.ApprovePurchaseOrder;
public class ApprovePurchaseOrderCommandValidator : AbstractValidator<ApprovePurchaseOrderCommand>
{
    public ApprovePurchaseOrderCommandValidator()
    {
        RuleFor(x => x.PurchaseOrderId)
            .NotEmpty()
            .WithMessage("Purchase order ID is required")
            .NotEqual(Guid.Empty)
            .WithMessage("Purchase order ID cannot be empty");
    }
}
