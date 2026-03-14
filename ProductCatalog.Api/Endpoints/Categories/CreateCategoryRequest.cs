namespace ProductCatalog.Api.Endpoints.Categories;

public sealed record CreateCategoryRequest(
    string Name,
    string? Description);