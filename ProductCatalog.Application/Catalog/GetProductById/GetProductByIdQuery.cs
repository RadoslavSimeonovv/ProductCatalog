using ProductCatalog.Application.Abstractions.Caching;
using ProductCatalog.Application.Catalog.Responses;

namespace ProductCatalog.Application.Catalog.GetProductById;

public sealed record GetProductByIdQuery(Guid Id)
    : ICachedQuery<ProductResponse>
{
    public string CacheKey => $"products:{Id}";
    public TimeSpan? Expiration => TimeSpan.FromMinutes(5);
}
