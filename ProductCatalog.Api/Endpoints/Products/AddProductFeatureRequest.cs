namespace ProductCatalog.Api.Endpoints.Products;

public sealed record AddProductFeatureRequest(
    string Name,
    string Value,
    int DisplayOrder);