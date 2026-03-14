namespace ProductCatalog.Api.Endpoints.Products;

public sealed record ChangeProductPriceRequest(
    MoneyRequestModel NewPrice);