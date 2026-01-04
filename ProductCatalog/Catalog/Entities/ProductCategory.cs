using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Catalog.ValueObjects;

namespace PlanFlow.Domain.Catalog.Entities;

/// <summary>
/// </summary>
public class ProductCategory : Entity
{
    public ProductCategory(Guid id)
        : base(id)
    {
    }
    public Name Name { get; private set; }
    public Description? Description { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
}
