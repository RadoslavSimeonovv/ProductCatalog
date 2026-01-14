using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Shared.ValueObjects;

namespace ProductCatalog.Domain.Order.Events;

/// <summary>
/// Raised when an order is submitted for payment.
/// </summary>
/// <param name="OrderId"></param>
/// <param name="Total"></param>
/// <param name="OccurredAtUtc"></param>
public sealed record OrderSubmittedForPaymentDomainEvent(
    Guid OrderId,
    Money Total,
    DateTime OccurredAtUtc) : IDomainEvent;