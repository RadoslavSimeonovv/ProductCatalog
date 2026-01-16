using ProductCatalog.Domain.Abstractions;

/// <summary>
/// Raised when a product becomes active and orderable.
/// </summary>
/// <param name="productId"></param>
/// <param name="occurredAtUtc"></param>
public sealed record ProductActivatedDomainEvent(
    Guid productId,
    DateTime occurredAtUtc) : IDomainEvent;