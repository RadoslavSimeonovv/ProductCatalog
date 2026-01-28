using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Catalog.Enums;
using ProductCatalog.Domain.Catalog.Errors;
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
        CreatedAt = createdAt;
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
    /// Attempts to change the product's status to the specified value. 
    /// </summary>
    /// <param name="newStatus">The new status to assign to the product.</param>
    /// <param name="nowUtc">The current date and time in UTC to record as the update timestamp.</param>
    /// <returns>A result indicating whether the status was changed. Returns a failure result if the new status is the same as
    /// the current status; otherwise, returns a success result.</returns>
    private Result ChangeStatus(ProductStatus newStatus, DateTime nowUtc)
    {
        if (Status == newStatus)
            return Result.Failure(ProductErrors.StatusUnchanged);

        Status = newStatus;
        UpdatedAt = nowUtc;

        return Result.Success();
    }

    /// <summary>
    /// Attempts to change the product's price to the specified value.
    /// </summary>
    /// <remarks>If the price is changed, a domain event is raised and the product's update timestamp is set
    /// to the current UTC time.</remarks>
    /// <param name="newPrice">The new price to assign to the product. Cannot be null.</param>
    /// <returns>A result indicating whether the price was successfully changed. Returns a failure result if the new price is
    /// invalid or unchanged.</returns>
    public Result ChangePrice(Money newPrice)
    {
        if (newPrice is null)
            return Result.Failure(ProductErrors.InvalidPrice);

        if (Price.Equals(newPrice))
            return Result.Failure(ProductErrors.PriceUnchanged);

        var oldPrice = Price;
        Price = newPrice;

        var dateTimeNow = DateTime.UtcNow;
        UpdatedAt = dateTimeNow;

        RaiseDomainEvent(new ProductPriceChangedDomainEvent(
            Id, oldPrice, newPrice, dateTimeNow));

        return Result.Success();
    }

    /// <summary>
    /// Attempts to publish the product by changing its status to active.
    /// </summary>
    /// <remarks>This method raises a domain event if the product is successfully published. The product can
    /// only be published if its current status is Draft or Inactive.</remarks>
    /// <returns>A <see cref="Result"/> indicating the outcome of the operation. Returns a failure result if the product is
    /// already active or in an invalid status; otherwise, returns a success result.</returns>
    public Result Publish()
    {
        if (Status == ProductStatus.Active)
        {
            return Result.Failure(ProductErrors.AlreadyActive);
        }
        if (Status != ProductStatus.Draft && Status != ProductStatus.Inactive)
        {
            return Result.Failure(ProductErrors.InvalidStatus);
        }

        var dateTimeNow = DateTime.UtcNow;

        var statusResult = ChangeStatus(ProductStatus.Active, dateTimeNow);

        if (statusResult.IsFailure)
            return statusResult;

        RaiseDomainEvent(new ProductActivatedDomainEvent(
            Id, dateTimeNow));

        return Result.Success();
    }

    /// <summary>
    /// Deactivates the product if it is currently active.
    /// </summary>
    /// <remarks>This method raises a domain event when the product is successfully deactivated. If the
    /// product is already inactive, no changes are made.</remarks>
    /// <returns>A <see cref="Result"/> indicating whether the deactivation was successful. Returns a failure result if the
    /// product is not active or if the status change fails.</returns>
    public Result Deactivate()
    {
        if (Status != ProductStatus.Active)
        {
            return Result.Failure(ProductErrors.NotActive);
        }

        var dateTimeNow = DateTime.UtcNow;

        var statusResult = ChangeStatus(ProductStatus.Inactive, dateTimeNow);

        if (statusResult.IsFailure)
            return statusResult;

        RaiseDomainEvent(new ProductDeactivatedDomainEvent(
            Id, dateTimeNow));

        return Result.Success();
    }

    /// <summary>
    /// Marks the product as discontinued if it is not already discontinued.
    /// </summary>
    /// <remarks>This method raises a domain event when the product is successfully discontinued. Once
    /// discontinued, the product's status cannot be reverted using this method.</remarks>
    /// <returns>A <see cref="Result"/> indicating whether the operation succeeded. Returns a failure result if the product is
    /// already discontinued.</returns>
    public Result Discontinue()
    {
        if (Status == ProductStatus.Discontinued)
        {
            return Result.Failure(ProductErrors.AlreadyDiscontinued);
        }

        var dateTimeNow = DateTime.UtcNow;

        var statusResult = ChangeStatus(ProductStatus.Discontinued, dateTimeNow);

        if (statusResult.IsFailure)
            return statusResult;

        RaiseDomainEvent(new ProductDiscontinuedDomainEvent(
            Id, dateTimeNow));

        return Result.Success();
    }

    /// <summary>
    /// Changes the category of the product to the specified category identifier.
    /// </summary>
    /// <remarks>This method updates the product's category and modification timestamp if the operation is
    /// valid. If the category is changed, a domain event is raised to signal the update. The operation cannot be
    /// performed if the product is discontinued.</remarks>
    /// <param name="newCategoryId">The unique identifier of the new category to assign to the product. Cannot be <see cref="Guid.Empty"/>.</param>
    /// <returns>A <see cref="Result"/> indicating whether the category change was successful. Returns a failure result if the
    /// new category identifier is invalid, unchanged, or if the product is discontinued.</returns>
    public Result ChangeCategory(Guid newCategoryId)
    {
        if (newCategoryId == Guid.Empty)
            return Result.Failure(ProductErrors.InvalidCategoryId);

        if (CategoryId == newCategoryId)
            return Result.Failure(ProductErrors.CategoryUnchanged);

        if (Status == ProductStatus.Discontinued)
            return Result.Failure(ProductErrors.DiscontinuedCannotBeModified);

        var oldCategoryId = CategoryId;
        var dateTimeNow = DateTime.UtcNow;

        CategoryId = newCategoryId;
        UpdatedAt = dateTimeNow;

        RaiseDomainEvent(new ProductCategoryChangedDomainEvent(
            Id, oldCategoryId, newCategoryId, dateTimeNow));

        return Result.Success();
    }

    /// <summary>
    /// Adds a new feature to the product with the specified identifier, name, value, and display order.
    /// </summary>
    /// <remarks>This method does not allow adding features to discontinued products. Feature names are
    /// compared case-insensitively to prevent duplicates.</remarks>
    /// <param name="featureId">The unique identifier for the feature to add. Must not be <see cref="Guid.Empty"/> and must not duplicate an
    /// existing feature's identifier.</param>
    /// <param name="name">The name of the feature. Cannot be null, empty, or whitespace, and must not duplicate an existing feature name
    /// (case-insensitive).</param>
    /// <param name="value">The value associated with the feature. Cannot be null, empty, or whitespace.</param>
    /// <param name="displayOrder">The position at which the feature should be displayed relative to other features.</param>
    /// <returns>A <see cref="Result"/> indicating whether the feature was successfully added. Returns a failure result if the
    /// input is invalid, the feature already exists, or the product cannot be modified.</returns>
    public Result AddFeature(Guid featureId, string name, string value, int displayOrder)
    {
        if (featureId == Guid.Empty)
            return Result.Failure(ProductErrors.InvalidFeatureId);

        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure(ProductErrors.InvalidFeatureName);

        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure(ProductErrors.InvalidFeatureValue);

        if (Status == ProductStatus.Discontinued)
            return Result.Failure(ProductErrors.DiscontinuedCannotBeModified);

        if (_features.Any(f => f.Id == featureId))
            return Result.Failure(ProductErrors.DuplicateFeatureId);

        var featureName = name.Trim();
        if (_features.Any(f => f.Name.Equals(featureName, StringComparison.OrdinalIgnoreCase)))
            return Result.Failure(ProductErrors.FeatureAlreadyExists);

        var featureValue = value.Trim();
        var dateTimeNow = DateTime.UtcNow;

        var feature = new ProductFeature(featureId, featureName, featureValue, displayOrder, Id, dateTimeNow);

        _features.Add(feature);

        UpdatedAt = dateTimeNow;

        RaiseDomainEvent(new ProductFeatureAddedDomainEvent(
            Id, feature.Id, feature.Name, feature.Value, feature.DisplayOrder, dateTimeNow));

        return Result.Success();
    }

    /// <summary>
    /// Updates the value of a product feature identified by the specified feature ID.
    /// </summary>
    /// <remarks>If the product status is discontinued, feature values cannot be modified. The method trims
    /// leading and trailing white space from the new value before updating. No update occurs if the new value is
    /// identical to the current value.</remarks>
    /// <param name="featureId">The unique identifier of the feature to update. Must not be <see cref="Guid.Empty"/>.</param>
    /// <param name="newValue">The new value to assign to the feature. Cannot be null, empty, or consist only of white-space characters.</param>
    /// <returns>A <see cref="Result"/> indicating the outcome of the operation. Returns a failure result if the feature ID is
    /// invalid, the new value is invalid, the product is discontinued, the feature is not found, or the new value is
    /// unchanged; otherwise, returns a success result.</returns>
    public Result UpdateFeatureValue(Guid featureId, string newValue)
    {
        if (featureId == Guid.Empty)
            return Result.Failure(ProductErrors.InvalidFeatureId);

        if (string.IsNullOrWhiteSpace(newValue))
            return Result.Failure(ProductErrors.InvalidFeatureValue);

        if (Status == ProductStatus.Discontinued)
            return Result.Failure(ProductErrors.DiscontinuedCannotBeModified);

        var feature = _features.SingleOrDefault(f => f.Id == featureId);

        if (feature is null)
            return Result.Failure(ProductErrors.FeatureNotFound);

        var trimmedNewValue = newValue.Trim();
        if (feature.Value == trimmedNewValue)
            return Result.Failure(ProductErrors.FeatureValueUnchanged);

        var oldValue = feature.Value;

        feature.UpdateValue(trimmedNewValue);

        var dateTimeNow = DateTime.UtcNow;
        UpdatedAt = dateTimeNow;

        RaiseDomainEvent(new ProductFeatureUpdatedDomainEvent(
            Id, feature.Id, feature.Name, oldValue, trimmedNewValue, dateTimeNow));

        return Result.Success();
    }

    /// <summary>
    /// Removes the feature with the specified identifier from the product.
    /// </summary>
    /// <remarks>If the product is discontinued, features cannot be removed. If the specified feature does not
    /// exist, the operation fails and no changes are made.</remarks>
    /// <param name="featureId">The unique identifier of the feature to remove. Cannot be <see cref="Guid.Empty"/>.</param>
    /// <returns>A <see cref="Result"/> indicating whether the feature was successfully removed. Returns a failure result if the
    /// feature does not exist, the identifier is invalid, or the product cannot be modified.</returns>
    public Result RemoveFeature(Guid featureId)
    {
        if (featureId == Guid.Empty)
            return Result.Failure(ProductErrors.InvalidFeatureId);

        if (Status == ProductStatus.Discontinued)
            return Result.Failure(ProductErrors.DiscontinuedCannotBeModified);

        var feature = _features.SingleOrDefault(f => f.Id == featureId);

        if (feature == null)
            return Result.Failure(ProductErrors.FeatureNotFound);

        _features.Remove(feature);

        var dateTimeNow = DateTime.UtcNow;
        UpdatedAt = dateTimeNow;

        RaiseDomainEvent(new ProductFeatureRemovedDomainEvent(
            Id, feature.Id, feature.Name, dateTimeNow));

        return Result.Success();
    }
}