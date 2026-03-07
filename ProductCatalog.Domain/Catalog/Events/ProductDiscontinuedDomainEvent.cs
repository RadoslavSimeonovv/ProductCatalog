using ProductCatalog.Domain.Abstractions;

namespace ProductCatalog.Domain.Catalog.Events;

/// <summary>
/// Raised when a product becomes discontinued permanently
/// </summary>
/// <param name="productId"></param>
/// <param name="occurredAtUtc"></param>
public sealed record ProductDiscontinuedDomainEvent(
    Guid productId,
    DateTime occurredAtUtc) : IDomainEvent;