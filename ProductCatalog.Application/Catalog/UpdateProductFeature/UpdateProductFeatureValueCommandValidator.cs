using FluentValidation;

namespace ProductCatalog.Application.Catalog.UpdateProductFeature;

public class UpdateProductFeatureValueCommandValidator : AbstractValidator<UpdateProductFeatureValueCommand>
{
    public UpdateProductFeatureValueCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required.");

        RuleFor(x => x.FeatureId)
            .NotEmpty().WithMessage("Feature ID is required.");

        RuleFor(x => x.NewValue)
            .NotEmpty().WithMessage("Feature value is required.")
            .MaximumLength(100).WithMessage("Feature value cannot exceed 100 characters.");
    }
}
