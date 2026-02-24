using FluentValidation;

namespace ProductCatalog.Application.Catalog.ChangeProductPrice;

public class ChangeProductPriceCommandValidator : AbstractValidator<ChangeProductPriceCommand>
{
    public ChangeProductPriceCommandValidator()
    {
        RuleFor(x => x.ProductId)
        .NotEmpty().WithMessage("Product ID is required.");

        RuleFor(x => x.NewPrice)
            .NotNull().WithMessage("New price is required.");

        RuleFor(x => x.NewPrice.Amount)
            .GreaterThan(0).WithMessage("Price must be greater than zero.");

        RuleFor(x => x.NewPrice.Currency)
            .NotNull().WithMessage("Currency is required.");

        RuleFor(x => x.NewPrice.Currency.Code)
            .NotEmpty()
            .Length(3)
            .WithMessage("Currency must be a 3-letter ISO code.");
    }
}