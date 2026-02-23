using ProductCatalog.Domain.Payment.Enums;

namespace ProductCatalog.Application.Payment;

public sealed class PaymentResponse
{
    public Guid PaymentId { get; init; }
    public Guid OrderId { get; init; }
    public required string CustomerId { get; init; }
    public decimal Amount { get; init; }
    public required string Currency { get; init; }
    public required string Provider { get; init; }
    public string? ProviderReference { get; init; }
    public required PaymentStatus Status { get; init; }
}
