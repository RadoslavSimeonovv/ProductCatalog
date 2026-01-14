using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Shared.ValueObjects;

namespace ProductCatalog.Domain.Order.Events;

/// <summary>
/// Raised when a new order is created.
/// </summary>
/// <param name="OrderId"></param>
/// <param name="CustomerEmail"></param>
/// <param name="Total"></param>
/// <param name="OccurredAtUtc"></param>
public sealed record OrderCreatedDomainEvent(
    Guid OrderId,
    string CustomerEmail,
    Money Total,
    DateTime OccurredAtUtc) : IDomainEvent;