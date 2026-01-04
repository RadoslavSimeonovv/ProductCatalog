using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Catalog.Enums;
using ProductCatalog.Domain.Catalog.ValueObjects;
using ProductCatalog.Domain.Shared.ValueObjects;

namespace PlanFlow.Domain.Catalog.Entities;

/// <summary>
/// </summary>
public class Product : Entity
{
    public Product(Guid id)
        : base(id)  
    {
    }
    public Name Name { get; private set; }
    public Description? Description { get; private set; }
    public Money Price { get; private set; }
    public ProductCategory Category { get; private set; }
    public Guid CategoryId { get; private set; }
    public Sku Sku { get; private set; }
    public ProductStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
}