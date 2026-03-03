using FluentValidation;
using ProductCatalog.Domain.Catalog.Enums;

namespace ProductCatalog.Application.Catalog.GetProducts;

public class GetProductsQueryValidator : AbstractValidator<GetProductsQuery>
{
    public GetProductsQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage("PageNumber must be >= 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("PageSize must be between 1 and 100.");

        RuleFor(x => x.CategoryId)
            .Must(id => id is null || id != Guid.Empty)
            .WithMessage("CategoryId must not be empty.");

        RuleFor(x => x.ProductStatus)
            .Must(s => s is null || Enum.IsDefined(typeof(ProductStatus), s))
            .WithMessage("ProductStatus is invalid.");

        RuleFor(x => x.SortBy)
            .Must(s => Enum.IsDefined(typeof(ProductSortBy), s))
            .WithMessage("SortBy is invalid.");

        RuleFor(x => x.SearchTerm)
            .MaximumLength(200)
            .WithMessage("SearchTerm cannot exceed 200 characters.")
            .Must(s => s is null || !string.IsNullOrWhiteSpace(s))
            .WithMessage("SearchTerm cannot be whitespace.")
            .When(x => x.SearchTerm is not null);
    }

}