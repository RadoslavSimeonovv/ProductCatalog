using ProductCatalog.Domain.Abstractions;

namespace ProductCatalog.Domain.Catalog.Events;

/// <summary>
/// Raised when product is moved from one category to another.
/// </summary>
/// <param name="ProductId"></param>
/// <param name="OldCategoryId"></param>
/// <param name="NewCategoryId"></param>
/// <param name="OccurredAtUtc"></param>
public sealed record ProductCategoryChangedDomainEvent(
    Guid ProductId,
    Guid OldCategoryId,
    Guid NewCategoryId,
    DateTime OccurredAtUtc) : IDomainEvent;