using ProductCatalog.Domain.Payment.Enums;

namespace ProductCatalog.Application.Payment;

public sealed record PaymentResponse(
    Guid PaymentId,
    Guid OrderId,
    string CustomerId,
    decimal Amount,
    string Currency,
    string Provider,
    string? ProviderReference,
    PaymentStatus Status);
