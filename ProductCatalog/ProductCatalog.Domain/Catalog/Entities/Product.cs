using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Catalog.Enums;
using ProductCatalog.Domain.Catalog.Events;
using ProductCatalog.Domain.Catalog.ValueObjects;
using ProductCatalog.Domain.Shared.ValueObjects;

namespace ProductCatalog.Domain.Catalog.Entities;

/// <summary>
/// </summary>
public sealed class Product : Entity
{
    private readonly List<ProductFeature> _features = new();
    public Product(Guid id,
        Name name,
        Description description,
        Money price,
        Guid categoryId,
        Sku sku,
        DateTime createdAt)
        : base(id)
    {
        Name = name;
        Description = description;
        Price = price;
        CategoryId = categoryId;
        Sku = sku;
        Status = ProductStatus.Draft;
    }
    public Name Name { get; private set; }
    public Description? Description { get; private set; }
    public Money Price { get; private set; }
    public ProductCategory? Category { get; private set; }
    public Guid CategoryId { get; private set; }
    public Sku Sku { get; private set; }
    public ProductStatus Status { get; private set; }
    public IReadOnlyCollection<ProductFeature> Features => _features.AsReadOnly();
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    /// <summary>
    /// Factory method to create a new Product
    /// </summary>
    /// <param name="name"></param>
    /// <param name="description"></param>
    /// <param name="price"></param>
    /// <param name="categoryId"></param>
    /// <param name="sku"></param>
    /// <returns></returns>
    public static Product Create(
        Name name,
        Description description,
        Money price,
        Guid categoryId,
        Sku sku)
    {
        var dateTimeNow = DateTime.UtcNow;

        var product = new Product(
            Guid.NewGuid(),
            name,
            description,
            price,
            categoryId,
            sku,
            dateTimeNow);

        product.RaiseDomainEvent(new ProductCreatedDomainEvent(
            product.Id, product.CategoryId, product.Sku.Value, dateTimeNow));
        return product;
    }

    /// <summary>
    /// Updates the product's status to the specified value and records the time of the change.
    /// </summary>
    /// <param name="newStatus">The new status to assign to the product. If the current status is already equal to this value, no update occurs.</param>
    /// <param name="nowUtc">The UTC date and time when the status change is recorded.</param>
    public void ChangeStatus(ProductStatus newStatus, DateTime nowUtc)
    {
        if (Status == newStatus)
            return;

        Status = newStatus;
        UpdatedAt = nowUtc;
    }

    /// <summary>
    /// Activates the product by setting its status to active and raising a domain event if the product is in a draft or
    /// inactive state.
    /// </summary>
    /// <remarks>This method has no effect if the product is already active or if its status is not draft or
    /// inactive. After activation, the product's update timestamp is set to the current UTC time, and a domain event is
    /// raised to signal the activation. This method should be called when the product is ready to be made
    /// available.</remarks>
    public void Publish()
    {
        if (Status == ProductStatus.Active)
        {
            return;
        }
        if (Status != ProductStatus.Draft && Status != ProductStatus.Inactive)
        {
            return;
        }

        var dateTimeNow = DateTime.UtcNow;

        ChangeStatus(ProductStatus.Active, dateTimeNow);

        RaiseDomainEvent(new ProductActivatedDomainEvent(
            Id, dateTimeNow));
    }

    /// <summary>
    /// Marks the product as inactive if it is currently active.
    /// </summary>
    /// <remarks>If the product is already inactive or not in the active state, this method has no effect.
    /// Calling this method updates the product's status and raises a domain event to signal the deactivation.</remarks>
    public void Deactivate()
    {
        if (Status != ProductStatus.Active)
        {
            return;
        }

        var dateTimeNow = DateTime.UtcNow;

        ChangeStatus(ProductStatus.Inactive, dateTimeNow);

        RaiseDomainEvent(new ProductDeactivatedDomainEvent(
            Id, dateTimeNow));
    }

    /// <summary>
    /// Marks the product as discontinued and raises a domain event to signal the change.
    /// </summary>
    /// <remarks>If the product is already discontinued, this method has no effect. After calling this method,
    /// the product's status is set to discontinued, the update timestamp is refreshed, and a domain event is raised to
    /// notify interested parties of the discontinuation.</remarks>
    public void Discontinue()
    {
        if (Status == ProductStatus.Discontinued)
        {
            return;
        }

        var dateTimeNow = DateTime.UtcNow;

        ChangeStatus(ProductStatus.Discontinued, dateTimeNow);

        RaiseDomainEvent(new ProductDiscontinuedDomainEvent(
            Id, dateTimeNow));
    }
}