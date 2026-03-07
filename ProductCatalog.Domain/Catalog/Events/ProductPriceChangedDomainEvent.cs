using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Shared.ValueObjects;

namespace ProductCatalog.Domain.Catalog.Events;

/// <summary>
/// Raised when the price of a product changes.
/// </summary>
public sealed record ProductPriceChangedDomainEvent(
    Guid productId,
    Money oldPrice,
    Money newPrice,
    DateTime occurredAtUtc) : IDomainEvent;