using FluentValidation;

namespace ProductCatalog.Application.Catalog.ChangeProductCategory;

public class ChangeProductCategoryCommandValidator : AbstractValidator<ChangeProductCategoryCommand>
{
    public ChangeProductCategoryCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required.");

        RuleFor(x => x.NewCategoryId)
            .NotEmpty().WithMessage("New category ID is required.");
    }
}