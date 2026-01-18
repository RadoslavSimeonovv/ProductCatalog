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
        string name,
        string description,
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
    public string Name { get; private set; }
    public string? Description { get; private set; }
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
        string name,
        string description,
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
    /// Updates the product's price to the specified value and records the change as a domain event.
    /// </summary>
    /// <remarks>This method updates the product's price and sets the last updated timestamp. It also raises a
    /// domain event to notify subscribers of the price change. No action is taken if the new price is equal to the
    /// current price.</remarks>
    /// <param name="newPrice">The new price to assign to the product. Must not be equal to the current price.</param>
    public void ChangePrice(Money newPrice)
    {
        if (newPrice is null)
            throw new ArgumentNullException(nameof(newPrice));

        if (Price == newPrice)
            return;

        var oldPrice = Price;
        Price = newPrice;

        var dateTimeNow = DateTime.UtcNow;
        UpdatedAt = dateTimeNow;

        RaiseDomainEvent(new ProductPriceChangedDomainEvent(
            Id, oldPrice, newPrice, dateTimeNow));
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

    /// <summary>
    /// Changes the category of the product to the specified category identifier.
    /// </summary>
    /// <remarks>Raises a domain event to signal that the product's category has changed. The product's last
    /// updated timestamp is also set to the current UTC time.</remarks>
    /// <param name="newCategoryId">The unique identifier of the new category to assign to the product. Must not be equal to the current category
    /// identifier.</param>
    public void ChangeCategory(Guid newCategoryId)
    {
        if (newCategoryId == Guid.Empty)
            throw new ArgumentException("CategoryId cannot be empty.", nameof(newCategoryId));

        if (CategoryId == newCategoryId)
            return;

        if (Status == ProductStatus.Discontinued)
            throw new InvalidOperationException("Discontinued products cannot change category.");

        var oldCategoryId = CategoryId;
        var dateTimeNow = DateTime.UtcNow;

        CategoryId = newCategoryId;
        UpdatedAt = dateTimeNow;

        RaiseDomainEvent(new ProductCategoryChangedDomainEvent(
            Id, oldCategoryId, newCategoryId, dateTimeNow));
    }

    /// <summary>
    /// Adds a new feature to the product with the specified identifier, name, value, and display order.
    /// </summary>
    /// <param name="featureId">The unique identifier of the feature to add. Cannot be <see cref="Guid.Empty"/>.</param>
    /// <param name="name">The name of the feature. Cannot be null or empty.</param>
    /// <param name="value">The value associated with the feature. Cannot be null or empty.</param>
    /// <param name="displayOrder">The position in which the feature should be displayed relative to other features.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="featureId"/> is <see cref="Guid.Empty"/>, or if <paramref name="name"/> or <paramref
    /// name="value"/> is null or empty.</exception>
    public void AddFeature(Guid featureId, string name, string value, int displayOrder)
    {
        if (featureId == Guid.Empty)
            throw new ArgumentException("FeatureId cannot be empty.", nameof(featureId));

        if (string.IsNullOrEmpty(name))
            throw new ArgumentException("Feature name cannot be null or empty.", nameof(name));

        if (string.IsNullOrEmpty(value))
            throw new ArgumentException("Feature value cannot be null or empty.", nameof(value));

        if (_features.Any(f => f.Id == featureId))
            throw new ArgumentException($"A feature with the ID '{featureId}' already exists for this product.", nameof(featureId));

        var feature = new ProductFeature(featureId, name, value, displayOrder, Id);

        _features.Add(feature);

        var dateTimeNow = DateTime.UtcNow;
        UpdatedAt = dateTimeNow;

        RaiseDomainEvent(new ProductFeatureAddedDomainEvent(
            Id, feature.Id, feature.Name, feature.Value, feature.DisplayOrder, dateTimeNow));
    }

    /// <summary>
    /// Updates the value of an existing feature identified by its unique identifier.   
    /// </summary>
    /// <remarks>If the new value is the same as the current value, no update is performed and no domain event
    /// is raised. The method updates the <c>UpdatedAt</c> property and raises a domain event when the feature value
    /// changes.</remarks>
    /// <param name="featureId">The unique identifier of the feature to update.</param>
    /// <param name="newValue">The new value to assign to the feature. Cannot be null or empty.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="newValue"/> is null or empty, or if a feature with the specified <paramref
    /// name="featureId"/> does not exist.</exception>
    public void UpdateFeatureValue(Guid featureId, string newValue)
    {
        if (featureId == Guid.Empty)
            throw new ArgumentException("FeatureId cannot be empty.", nameof(featureId));

        if (string.IsNullOrWhiteSpace(newValue))
            throw new ArgumentException("Feature value cannot be null or empty.", nameof(newValue));

        var feature = _features.SingleOrDefault(f => f.Id == featureId);

        if (feature == null)
            throw new InvalidOperationException($"Feature with ID '{featureId}' not found.");

        var trimmedNewValue = newValue.Trim();
        if (feature.Value == trimmedNewValue)
            return;

        var oldValue = feature.Value;

        feature.UpdateValue(newValue);

        var dateTimeNow = DateTime.UtcNow;
        UpdatedAt = dateTimeNow;

        RaiseDomainEvent(new ProductFeatureUpdatedDomainEvent(
            Id, feature.Id, feature.Name, oldValue, newValue, dateTimeNow));
    }

    /// <summary>
    /// Removes the feature with the specified identifier from the product.
    /// </summary>
    /// <param name="featureId">The unique identifier of the feature to remove. Cannot be empty.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="featureId"/> is empty or if no feature with the specified identifier exists.</exception>
    public void RemoveFeature(Guid featureId)
    {
        if (featureId == Guid.Empty)
            throw new ArgumentException("FeatureId cannot be empty.", nameof(featureId));

        var feature = _features.SingleOrDefault(f => f.Id == featureId);

        if (feature == null)
            throw new ArgumentException($"Feature with ID '{featureId}' not found.", nameof(featureId));

        _features.Remove(feature);

        var dateTimeNow = DateTime.UtcNow;
        UpdatedAt = dateTimeNow;

        RaiseDomainEvent(new ProductFeatureRemovedDomainEvent(
            Id, feature.Id, feature.Name, dateTimeNow));
    }
}