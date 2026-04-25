using ProductCatalog.Application.Abstractions.Caching;
using ProductCatalog.Application.Catalog.Responses;

namespace ProductCatalog.Application.Catalog.GetAllCategories;

public sealed record GetAllCategoriesQuery
    : ICachedQuery<IReadOnlyList<ProductCategoryResponse>>
{
    public string CacheKey => "categories:all";
    public TimeSpan? Expiration => TimeSpan.FromMinutes(10);
}
