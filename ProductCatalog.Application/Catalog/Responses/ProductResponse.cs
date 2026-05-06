namespace ProductCatalog.Application.Catalog.Responses;

public sealed record ProductResponse(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    string Currency,
    string? Sku,
    string Status,
    Guid? CategoryId);
