namespace ProductCatalog.Api.Endpoints.Categories;

public sealed record UpdateCategoryRequest(
    string Name,
    string Description);