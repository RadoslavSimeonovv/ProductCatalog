namespace ProductCatalog.Api.Endpoints.Payments;

public sealed record CreatePaymentRequest(Guid OrderId,
    string Provider,
    string? ProviderReference,
    string IdempotencyKey);