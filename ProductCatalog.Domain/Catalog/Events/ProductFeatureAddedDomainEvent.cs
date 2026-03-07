using ProductCatalog.Domain.Abstractions;

namespace ProductCatalog.Domain.Catalog.Events;

/// <summary>
/// Raised when a feature is added to a product.
/// </summary>
/// <param name="ProductId"></param>
/// <param name="FeatureId"></param>
/// <param name="Name"></param>
/// <param name="Value"></param>
/// <param name="DisplayOrder"></param>
/// <param name="OccurredAtUtc"></param>
public sealed record ProductFeatureAddedDomainEvent(
    Guid ProductId,
    Guid FeatureId,
    string Name,
    string Value,
    int DisplayOrder,
    DateTime OccurredAtUtc) : IDomainEvent;