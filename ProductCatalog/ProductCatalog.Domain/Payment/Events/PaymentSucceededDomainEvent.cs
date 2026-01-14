using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Shared.ValueObjects;

namespace ProductCatalog.Domain.Payment.Events;

/// <summary>
/// Raised when a payment succeeds.
/// </summary>
/// <param name="PaymentId"></param>
/// <param name="OrderId"></param>
/// <param name="Amount"></param>
/// <param name="ProviderReference"></param>
/// <param name="OccurredAtUtc"></param>
public sealed record PaymentSucceededDomainEvent(
    Guid PaymentId,
    Guid OrderId,
    Money Amount,
    string ProviderReference,
    DateTime OccurredAtUtc) : IDomainEvent;