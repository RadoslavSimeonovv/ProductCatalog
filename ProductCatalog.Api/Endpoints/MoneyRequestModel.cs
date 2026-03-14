namespace ProductCatalog.Api.Endpoints;

public sealed record MoneyRequestModel(decimal Amount,
    string Currency);