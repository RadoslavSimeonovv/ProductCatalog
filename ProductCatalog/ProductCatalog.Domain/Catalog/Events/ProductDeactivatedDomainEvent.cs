using ProductCatalog.Domain.Abstractions;

namespace ProductCatalog.Domain.Catalog.Events;

/// <summary>
/// Raised when a product is deactivated and becomes non-orderable.
/// </summary>
/// <param name="productId"></param>
/// <param name="occurredAtUtc"></param>
public sealed record ProductDeactivatedDomainEvent(
    Guid productId, 
    DateTime occurredAtUtc) : IDomainEvent;