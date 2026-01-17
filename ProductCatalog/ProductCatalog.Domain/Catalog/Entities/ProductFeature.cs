using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Catalog.ValueObjects;

namespace ProductCatalog.Domain.Catalog.Entities;

/// <summary>

/// </summary>
public sealed class ProductFeature : Entity
{
    public ProductFeature(Guid id,
        string name,
        string value,
        int displayOrder,
        Guid productId)
        : base(id)
    {
        Name = name;
        Value = value;
        DisplayOrder = displayOrder;
        ProductId = productId;
    }

    public string Name { get; private set; }
    public string Value { get; private set; }
    public int DisplayOrder { get; private set; }
    public Product? Product { get; private set; }
    public Guid ProductId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
}
