using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Shared.ValueObjects;

namespace ProductCatalog.Domain.Payment.Events;

/// <summary>
/// Raised when a payment fails.
/// </summary>
/// <param name="PaymentId"></param>
/// <param name="OrderId"></param>
/// <param name="Amount"></param>
/// <param name="FailureReason"></param>
/// <param name="OccurredAtUtc"></param>
public sealed record PaymentFailedDomainEvent(Guid PaymentId,
    Guid OrderId,
    Money Amount,
    string? FailureReason,
    DateTime OccurredAtUtc) : IDomainEvent;