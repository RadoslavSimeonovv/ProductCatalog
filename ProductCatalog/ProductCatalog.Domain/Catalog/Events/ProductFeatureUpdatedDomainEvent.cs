using ProductCatalog.Domain.Abstractions;

namespace ProductCatalog.Domain.Catalog.Events;

/// <summary>
/// Raised when a feature is updated.
/// </summary>
/// <param name="ProductId"></param>
/// <param name="FeatureId"></param>
/// <param name="Name"></param>
/// <param name="OldValue"></param>
/// <param name="NewValue"></param>
/// <param name="OccurredAtUtc"></param>
public sealed record ProductFeatureUpdatedDomainEvent(
    Guid ProductId,
    Guid FeatureId,
    string Name,
    string OldValue,
    string NewValue,
    DateTime OccurredAtUtc) : IDomainEvent;