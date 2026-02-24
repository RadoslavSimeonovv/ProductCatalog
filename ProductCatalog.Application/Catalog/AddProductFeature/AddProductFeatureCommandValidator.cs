using FluentValidation;

namespace ProductCatalog.Application.Catalog.AddProductFeature;

public class AddProductFeatureCommandValidator : AbstractValidator<AddProductFeatureCommand>
{
    public AddProductFeatureCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required.");

        RuleFor(x => x.FeatureId)
           .NotEmpty().WithMessage("Feature ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Feature name is required.")
            .MaximumLength(100).WithMessage("Feature name cannot exceed 100 characters.");

        RuleFor(x => x.Value)
            .NotEmpty().WithMessage("Feature value is required.")
            .MaximumLength(100).WithMessage("Feature value cannot exceed 100 characters.");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("Display order must be a non-negative integer.");
    }
}