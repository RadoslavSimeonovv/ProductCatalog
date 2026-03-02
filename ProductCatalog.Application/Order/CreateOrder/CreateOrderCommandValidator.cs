using FluentValidation;

namespace ProductCatalog.Application.Order.CreateOrder;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("CustomerId is required.")
            .MaximumLength(100).WithMessage("CustomerId cannot exceed 100 characters.");

        RuleFor(x => x.CustomerEmail)
            .NotEmpty().WithMessage("Customer email is required.")
            .EmailAddress().WithMessage("Customer email is not valid.")
            .MaximumLength(256).WithMessage("Customer email cannot exceed 256 characters.");

        RuleFor(x => x.Items)
            .NotNull().WithMessage("Items are required.")
            .NotEmpty().WithMessage("Order must contain at least one item.")
            .Must(items => items.Count <= 200).WithMessage("Order cannot contain more than 200 items.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId)
                .NotEmpty().WithMessage("ProductId is required.");

            item.RuleFor(i => i.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than zero.");

            item.RuleFor(i => i.Currency)
                .NotEmpty().WithMessage("Currency is required.")
                .Length(3).WithMessage("Currency must be a 3-letter ISO code.");

            item.RuleFor(i => i.UnitPriceAmount)
                .GreaterThanOrEqualTo(0).WithMessage("Unit price cannot be negative.");
        });
    }
}