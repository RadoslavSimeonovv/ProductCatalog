namespace ProductCatalog.Api.Endpoints.Payments;

public sealed record MarkPaymentFailedRequest(
    string? Reason);