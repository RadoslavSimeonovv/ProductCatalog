using FluentValidation;

namespace ProductCatalog.Application.Catalog.GetProductFeatures;

public class GetProductFeaturesQueryValidator : AbstractValidator<GetProductFeaturesQuery>
{
    public GetProductFeaturesQueryValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product ID must not be empty.");
    }
}