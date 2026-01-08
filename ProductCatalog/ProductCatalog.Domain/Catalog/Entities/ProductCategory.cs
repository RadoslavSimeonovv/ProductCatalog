using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Catalog.ValueObjects;

namespace PlanFlow.Domain.Catalog.Entities;

/// <summary>
/// </summary>
public sealed class ProductCategory : Entity
{
    public ProductCategory(Guid id,
        Name name,
        Description description)
        : base(id)
    {
        Name = name;
        Description = description;
    }
    public Name Name { get; private set; }
    public Description? Description { get; private set; }

    private readonly List<Product> _products = new();
    public IReadOnlyCollection<Product> Products => _products.AsReadOnly();
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
}
