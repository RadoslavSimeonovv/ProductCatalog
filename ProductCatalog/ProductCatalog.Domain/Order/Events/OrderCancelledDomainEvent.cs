using ProductCatalog.Domain.Abstractions;

namespace ProductCatalog.Domain.Order.Events;

/// <summary>
/// Raised when an order is cancelled.
/// </summary>
/// <param name="OrderId"></param>
/// <param name="Reason"></param>
/// <param name="OccurredAtUtc"></param>
public sealed record OrderCancelledDomainEvent(
    Guid OrderId,
    string? Reason,
    DateTime OccurredAtUtc) : IDomainEvent;