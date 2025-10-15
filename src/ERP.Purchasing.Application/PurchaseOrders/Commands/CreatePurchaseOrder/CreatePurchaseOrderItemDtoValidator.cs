using ERP.Purchasing.Application.Common.DTOs;
using FluentValidation;

namespace ERP.Purchasing.Application.PurchaseOrders.Commands.CreatePurchaseOrder;
public class CreatePurchaseOrderItemDtoValidator : AbstractValidator<CreatePurchaseOrderItemDto>
{
    public CreatePurchaseOrderItemDtoValidator()
    {
        RuleFor(x => x.GoodCode)
            .NotEmpty()
            .WithMessage("Good code is required")
            .MaximumLength(50)
            .WithMessage("Good code cannot exceed 50 characters")
            .Matches(@"^[A-Z0-9_-]+$")
            .WithMessage("Good code must contain only uppercase letters, numbers, hyphens, and underscores");

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Price must be greater than zero")
            .LessThanOrEqualTo(999999.99m)
            .WithMessage("Price cannot exceed 999999.99");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .WithMessage("Currency is required")
            .Length(3)
            .WithMessage("Currency must be exactly 3 characters (ISO 4217 code)")
            .Matches(@"^[A-Z]{3}$")
            .WithMessage("Currency must be uppercase ISO 4217 code");
    }
}
