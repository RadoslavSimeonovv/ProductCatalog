using ProductCatalog.Domain.Abstractions;

namespace ProductCatalog.Domain.Catalog.Events;

/// <summary>
/// Raised when a new product is created
/// </summary>
/// <param name="productId"></param>
/// <param name="occurredAtUtc"></param>
public sealed record ProductCreatedDomainEvent(
    Guid productId,
    DateTime occurredAtUtc) : IDomainEvent;