using ProductCatalog.Application.Abstractions.Caching;
using ProductCatalog.Application.Catalog.Responses;

namespace ProductCatalog.Application.Catalog.GetCategoryById;

public sealed record GetCategoryByIdQuery(Guid Id)
    : ICachedQuery<ProductCategoryResponse>
{
    public string CacheKey => $"categories:{Id}";
    public TimeSpan? Expiration => TimeSpan.FromMinutes(10);
}
