namespace ProductCatalog.Application.Catalog.Responses;

public sealed class ProductFeatureResponse
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Value { get; init; }
    public int DisplayOrder { get; init; }
}
