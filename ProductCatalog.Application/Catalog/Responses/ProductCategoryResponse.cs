namespace ProductCatalog.Application.Catalog.Responses;

public sealed class ProductCategoryResponse
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
}