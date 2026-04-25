using ProductCatalog.Application.Abstractions.Caching;

namespace ProductCatalog.Application.Catalog.GetProductFeatures;

public sealed record GetProductFeaturesQuery(Guid ProductId)
    : ICachedQuery<GetProductFeaturesQueryResponse>
{
    public string CacheKey => $"products:{ProductId}:features";
    public TimeSpan? Expiration => TimeSpan.FromMinutes(5);
}
