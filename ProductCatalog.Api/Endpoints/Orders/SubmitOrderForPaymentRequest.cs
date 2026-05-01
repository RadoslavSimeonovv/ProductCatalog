namespace ProductCatalog.Api.Endpoints.Orders;

public sealed record SubmitOrderForPaymentRequest(string Provider, string IdempotencyKey);
