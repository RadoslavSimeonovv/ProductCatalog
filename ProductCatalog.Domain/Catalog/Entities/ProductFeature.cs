using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Catalog.Errors;

namespace ProductCatalog.Domain.Catalog.Entities;

/// <summary>

/// </summary>
public sealed class ProductFeature : Entity
{
    internal ProductFeature(Guid id,
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

    private ProductFeature() { }

    public string Name { get; private set; }
    public string Value { get; private set; }
    public int DisplayOrder { get; private set; }
    public Product? Product { get; private set; }
    public Guid ProductId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    internal Result UpdateValue(string newValue)
    {
        if (string.IsNullOrEmpty(newValue))
            return Result.Failure(ProductErrors.InvalidFeatureValue);

        Value = newValue;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }
}