using ProductCatalog.Application.Catalog.Responses;

namespace ProductCatalog.Application.Catalog.GetProductFeatures;

public sealed record GetProductFeaturesQueryResponse(
    Guid ProductId,
    string ProductName,
    IReadOnlyList<ProductFeatureResponse> Features);
