namespace ProductCatalog.Application.Catalog.Responses;

public sealed record ProductFeatureResponse(
    Guid Id,
    string Name,
    string Value,
    int DisplayOrder);
