using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Catalog.ValueObjects;

namespace ProductCatalog.Domain.Catalog.Entities;

/// <summary>
/// </summary>
public sealed class ProductCategory : Entity
{
    private readonly List<Product> _products = new();
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
    public IReadOnlyCollection<Product> Products => _products.AsReadOnly();
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
}
