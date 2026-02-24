using FluentValidation;

namespace ProductCatalog.Application.Catalog.RemoveProductFeature;

public class RemoveProductFeatureCommandValidator : AbstractValidator<RemoveProductFeatureCommand>
{
    public RemoveProductFeatureCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required.");

        RuleFor(x => x.FeatureId)
            .NotEmpty().WithMessage("Feature ID is required.");
    }
}