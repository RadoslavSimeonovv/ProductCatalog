using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Catalog.Enums;
using ProductCatalog.Domain.Catalog.ValueObjects;
using ProductCatalog.Domain.Shared.ValueObjects;

namespace PlanFlow.Domain.Catalog.Entities;

/// <summary>
/// </summary>
public sealed class Product : Entity
{
    public Product(Guid id,
        Name name,
        Description description,
        Money price,
        Guid categoryId,
        Sku sku,
        ProductStatus status)
        : base(id)  
    {
        Name = name;
        Description = description;
        Price = price;
        CategoryId = categoryId;
        Sku = sku;
        Status = status;
    }
    public Name Name { get; private set; }
    public Description? Description { get; private set; }
    public Money Price { get; private set; }
    public ProductCategory? Category { get; private set; }
    public Guid CategoryId { get; private set; }
    public Sku Sku { get; private set; }
    public ProductStatus Status { get; private set; }

    private readonly List<ProductFeature> _features = new();
    public IReadOnlyCollection<ProductFeature> Features => _features.AsReadOnly();
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
}