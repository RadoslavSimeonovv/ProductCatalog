using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Shared.ValueObjects;

namespace ProductCatalog.Domain.Payment.Events;

/// <summary>
/// Raised when a payment is initiated.
/// </summary>
/// <param name="PaymentId"></param>
/// <param name="OrderId"></param>
/// <param name="Amount"></param>
/// <param name="Provider"></param>
/// <param name="IdempotencyKey"></param>
/// <param name="OccurredAtUtc"></param>
public sealed record PaymentInitiatedDomainEvent(
    Guid PaymentId,
    Guid OrderId,
    Money Amount,
    string Provider,
    string IdempotencyKey,
    DateTime OccurredAtUtc) : IDomainEvent;