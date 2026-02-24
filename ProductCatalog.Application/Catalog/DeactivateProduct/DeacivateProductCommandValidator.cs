using FluentValidation;

namespace ProductCatalog.Application.Catalog.DeactivateProduct;

public class DeacivateProductCommandValidator : AbstractValidator<DeactiveProductCommand>
{
    public DeacivateProductCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required.");
    }
}