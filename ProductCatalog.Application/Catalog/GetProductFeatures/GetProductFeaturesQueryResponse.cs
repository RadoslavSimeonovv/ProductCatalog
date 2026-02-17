using ProductCatalog.Application.Catalog.Responses;

namespace ProductCatalog.Application.Catalog.GetProductFeatures;

public sealed class GetProductFeaturesQueryResponse
{
    public Guid ProductId { get; init; }
    public required string ProductName { get; init; }
    public required IReadOnlyList<ProductFeatureResponse> Features { get; init; }
}
