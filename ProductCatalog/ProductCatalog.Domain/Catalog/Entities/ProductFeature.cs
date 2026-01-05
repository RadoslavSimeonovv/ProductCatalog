using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Catalog.ValueObjects;

namespace PlanFlow.Domain.Catalog.Entities;

/// <summary>

/// </summary>
public class ProductFeature : Entity
{
    public ProductFeature(Guid id)
        : base(id)
    {

    }

    public Name Name { get; private set; }
    public Description Description { get; private set; }
    public Product Product { get; private set; }
    public Guid ProductId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
}
