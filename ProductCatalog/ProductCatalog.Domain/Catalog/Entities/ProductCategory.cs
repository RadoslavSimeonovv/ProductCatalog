using ProductCatalog.Domain.Abstractions;

namespace ProductCatalog.Domain.Catalog.Entities;

/// <summary>
/// </summary>
public sealed class ProductCategory : Entity
{
    private readonly List<Product> _products = new();
    public ProductCategory(Guid id,
        string name,
        string description)
        : base(id)
    {
        Name = name;
        Description = description;
    }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public IReadOnlyCollection<Product> Products => _products.AsReadOnly();
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
}
