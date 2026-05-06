namespace ProductCatalog.Application.Catalog.Responses;

public sealed record ProductCategoryResponse(
    Guid Id,
    string Name,
    string? Description);
