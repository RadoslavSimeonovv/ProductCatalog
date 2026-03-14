namespace ProductCatalog.Api.Endpoints.Products;

public sealed record CreateProductRequest(
    string Name,
    string? Description,
    MoneyRequestModel Price,
    Guid CategoryId,
    string Sku);