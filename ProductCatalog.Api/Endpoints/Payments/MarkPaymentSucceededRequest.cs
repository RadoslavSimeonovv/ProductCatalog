namespace ProductCatalog.Api.Endpoints.Payments;

public sealed record MarkPaymentSucceededRequest(
    string? ProviderReference);