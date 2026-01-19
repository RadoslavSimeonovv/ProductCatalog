using ProductCatalog.Domain.Abstractions;

namespace ProductCatalog.Domain.Catalog.Entities;

/// <summary>

/// </summary>
public sealed class ProductFeature : Entity
{
    public ProductFeature(Guid id,
        string name,
        string value,
        int displayOrder,
        Guid productId,
        DateTime createdAt)
        : base(id)
    {
        Name = name;
        Value = value;
        DisplayOrder = displayOrder;
        ProductId = productId;
        CreatedAt = createdAt;
    }

    public string Name { get; private set; }
    public string Value { get; private set; }
    public int DisplayOrder { get; private set; }
    public Product? Product { get; private set; }
    public Guid ProductId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    internal void UpdateValue(string newValue)
    {
        if (string.IsNullOrEmpty(newValue))
            throw new ArgumentException("Feature value cannot be null or empty.", nameof(newValue));
        
        Value = newValue;
    }
}