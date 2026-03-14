namespace ProductCatalog.Api.Endpoints.Products;

public sealed record ChangeProductCategoryRequest(
    Guid NewCategoryId);