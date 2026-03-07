using ProductCatalog.Domain.Abstractions;

namespace ProductCatalog.Domain.Catalog.Events;

/// <summary>
/// Raised when a feature is removed from a product.
/// </summary>
/// <param name="ProductId"></param>
/// <param name="FeatureId"></param>
/// <param name="Name"></param>
/// <param name="OccurredAtUtc"></param>
public sealed record ProductFeatureRemovedDomainEvent(
    Guid ProductId,
    Guid FeatureId,
    string Name,
    DateTime OccurredAtUtc) : IDomainEvent;