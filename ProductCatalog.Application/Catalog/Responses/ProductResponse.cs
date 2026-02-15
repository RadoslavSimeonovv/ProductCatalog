namespace ProductCatalog.Application.Catalog.Responses;

public sealed class ProductResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public required string Currency { get; init; }
    public string? Sku { get; init; }
}