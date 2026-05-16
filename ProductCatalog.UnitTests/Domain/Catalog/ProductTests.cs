using FluentAssertions;
using ProductCatalog.Domain.Catalog.Entities;
using ProductCatalog.Domain.Catalog.Enums;
using ProductCatalog.Domain.Catalog.Errors;
using ProductCatalog.Domain.Catalog.Events;

namespace ProductCatalog.UnitTests.Domain.Catalog;

public class ProductTests
{
    #region Create

    [Fact]
    public void Create_Should_ReturnFailure_WhenNameIsInvalid()
    {
        // Act
        var result = Product.Create(ProductData.InvalidName, ProductData.Description, ProductData.Price, ProductData.CategoryId, ProductData.Sku);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.InvalidName);
    }

    [Fact]
    public void Create_Should_ReturnFailure_WhenPriceIsNull()
    {
        // Act
        var result = Product.Create(ProductData.Name, ProductData.Description, null!, ProductData.CategoryId, ProductData.Sku);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.InvalidPrice);
    }

    [Fact]
    public void Create_Should_ReturnFailure_WhenCategoryIdIsEmptyGuid()
    {
        // Act
        var result = Product.Create(ProductData.Name, ProductData.Description, ProductData.Price, Guid.Empty, ProductData.Sku);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.InvalidCategoryId);
    }

    [Fact]
    public void Create_Should_ReturnSuccess_WhenAllParametersAreValid()
    {
        // Act
        var result = Product.Create(ProductData.Name, ProductData.Description, ProductData.Price, ProductData.CategoryId, ProductData.Sku);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be(ProductData.Name);
        result.Value.Description.Should().Be(ProductData.Description);
        result.Value.Price.Should().Be(ProductData.Price);
        result.Value.CategoryId.Should().Be(ProductData.CategoryId);
        result.Value.Sku.Should().Be(ProductData.Sku);
        result.Value.Status.Should().Be(ProductStatus.Draft);
    }

    [Fact]
    public void Create_Should_RaiseDomainEvent_WhenProductIsCreated()
    {
        // Act
        var result = Product.Create(ProductData.Name, ProductData.Description, ProductData.Price, ProductData.CategoryId, ProductData.Sku);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var evt = result.Value.GetDomainEvents().OfType<ProductCreatedDomainEvent>().SingleOrDefault();
        evt.Should().NotBeNull();
        evt!.productId.Should().Be(result.Value.Id);
        evt.categoryId.Should().Be(ProductData.CategoryId);
        evt.sku.Should().Be(ProductData.Sku.Value);
    }

    #endregion

    #region Publish

    [Fact]
    public void Publish_Should_ReturnSuccess_WhenProductIsInDraft()
    {
        // Arrange
        var product = ProductData.CreateProduct();

        // Act
        var result = product.Publish();

        // Assert
        result.IsSuccess.Should().BeTrue();
        product.Status.Should().Be(ProductStatus.Active);
    }

    [Fact]
    public void Publish_Should_ReturnSuccess_WhenProductIsInactive()
    {
        // Arrange
        var product = ProductData.CreateActiveProduct();
        product.Deactivate();

        // Act
        var result = product.Publish();

        // Assert
        result.IsSuccess.Should().BeTrue();
        product.Status.Should().Be(ProductStatus.Active);
    }

    [Fact]
    public void Publish_Should_ReturnFailure_WhenProductIsAlreadyActive()
    {
        // Arrange
        var product = ProductData.CreateActiveProduct();

        // Act
        var result = product.Publish();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.AlreadyActive);
    }

    [Fact]
    public void Publish_Should_ReturnFailure_WhenProductIsDiscontinued()
    {
        // Arrange
        var product = ProductData.CreateDiscontinuedProduct();

        // Act
        var result = product.Publish();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.InvalidStatus);
    }

    [Fact]
    public void Publish_Should_RaiseDomainEvent_WhenProductIsPublished()
    {
        // Arrange
        var product = ProductData.CreateProduct();

        // Act
        product.Publish();

        // Assert
        var evt = product.GetDomainEvents().OfType<ProductActivatedDomainEvent>().SingleOrDefault();
        evt.Should().NotBeNull();
        evt!.productId.Should().Be(product.Id);
    }

    #endregion

    #region Deactivate

    [Fact]
    public void Deactivate_Should_ReturnSuccess_WhenProductIsActive()
    {
        // Arrange
        var product = ProductData.CreateActiveProduct();

        // Act
        var result = product.Deactivate();

        // Assert
        result.IsSuccess.Should().BeTrue();
        product.Status.Should().Be(ProductStatus.Inactive);
    }

    [Fact]
    public void Deactivate_Should_ReturnFailure_WhenProductIsNotActive()
    {
        // Arrange
        var product = ProductData.CreateProduct();

        // Act
        var result = product.Deactivate();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.NotActive);
    }

    [Fact]
    public void Deactivate_Should_RaiseDomainEvent_WhenProductIsDeactivated()
    {
        // Arrange
        var product = ProductData.CreateActiveProduct();

        // Act
        product.Deactivate();

        // Assert
        var evt = product.GetDomainEvents().OfType<ProductDeactivatedDomainEvent>().SingleOrDefault();
        evt.Should().NotBeNull();
        evt!.productId.Should().Be(product.Id);
    }

    #endregion

    #region Discontinue

    [Fact]
    public void Discontinue_Should_ReturnSuccess_WhenProductIsNotDiscontinued()
    {
        // Arrange
        var product = ProductData.CreateProduct();

        // Act
        var result = product.Discontinue();

        // Assert
        result.IsSuccess.Should().BeTrue();
        product.Status.Should().Be(ProductStatus.Discontinued);
    }

    [Fact]
    public void Discontinue_Should_ReturnFailure_WhenProductIsAlreadyDiscontinued()
    {
        // Arrange
        var product = ProductData.CreateDiscontinuedProduct();

        // Act
        var result = product.Discontinue();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.AlreadyDiscontinued);
    }

    [Fact]
    public void Discontinue_Should_RaiseDomainEvent_WhenProductIsDiscontinued()
    {
        // Arrange
        var product = ProductData.CreateProduct();

        // Act
        product.Discontinue();

        // Assert
        var evt = product.GetDomainEvents().OfType<ProductDiscontinuedDomainEvent>().SingleOrDefault();
        evt.Should().NotBeNull();
        evt!.productId.Should().Be(product.Id);
    }

    #endregion

    #region ChangePrice

    [Fact]
    public void ChangePrice_Should_ReturnFailure_WhenPriceIsNull()
    {
        // Arrange
        var product = ProductData.CreateProduct();

        // Act
        var result = product.ChangePrice(null!);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.InvalidPrice);
    }

    [Fact]
    public void ChangePrice_Should_ReturnFailure_WhenPriceIsUnchanged()
    {
        // Arrange
        var product = ProductData.CreateProduct();

        // Act
        var result = product.ChangePrice(ProductData.Price);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.PriceUnchanged);
    }

    [Fact]
    public void ChangePrice_Should_ReturnSuccess_WhenPriceIsValid()
    {
        // Arrange
        var product = ProductData.CreateProduct();

        // Act
        var result = product.ChangePrice(ProductData.NewPrice);

        // Assert
        result.IsSuccess.Should().BeTrue();
        product.Price.Should().Be(ProductData.NewPrice);
    }

    [Fact]
    public void ChangePrice_Should_RaiseDomainEvent_WhenPriceIsChanged()
    {
        // Arrange
        var product = ProductData.CreateProduct();

        // Act
        product.ChangePrice(ProductData.NewPrice);

        // Assert
        var evt = product.GetDomainEvents().OfType<ProductPriceChangedDomainEvent>().SingleOrDefault();
        evt.Should().NotBeNull();
        evt!.productId.Should().Be(product.Id);
        evt.oldPrice.Should().Be(ProductData.Price);
        evt.newPrice.Should().Be(ProductData.NewPrice);
    }

    #endregion

    #region ChangeCategory

    [Fact]
    public void ChangeCategory_Should_ReturnFailure_WhenCategoryIdIsEmpty()
    {
        // Arrange
        var product = ProductData.CreateProduct();

        // Act
        var result = product.ChangeCategory(Guid.Empty);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.InvalidCategoryId);
    }

    [Fact]
    public void ChangeCategory_Should_ReturnFailure_WhenCategoryIsUnchanged()
    {
        // Arrange
        var product = ProductData.CreateProduct();

        // Act
        var result = product.ChangeCategory(ProductData.CategoryId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.CategoryUnchanged);
    }

    [Fact]
    public void ChangeCategory_Should_ReturnFailure_WhenProductIsDiscontinued()
    {
        // Arrange
        var product = ProductData.CreateDiscontinuedProduct();

        // Act
        var result = product.ChangeCategory(ProductData.NewCategoryId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.DiscontinuedCannotBeModified);
    }

    [Fact]
    public void ChangeCategory_Should_ReturnSuccess_WhenCategoryIdIsValid()
    {
        // Arrange
        var product = ProductData.CreateProduct();

        // Act
        var result = product.ChangeCategory(ProductData.NewCategoryId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        product.CategoryId.Should().Be(ProductData.NewCategoryId);
    }

    [Fact]
    public void ChangeCategory_Should_RaiseDomainEvent_WhenCategoryIsChanged()
    {
        // Arrange
        var product = ProductData.CreateProduct();

        // Act
        product.ChangeCategory(ProductData.NewCategoryId);

        // Assert
        var evt = product.GetDomainEvents().OfType<ProductCategoryChangedDomainEvent>().SingleOrDefault();
        evt.Should().NotBeNull();
        evt!.ProductId.Should().Be(product.Id);
        evt.OldCategoryId.Should().Be(ProductData.CategoryId);
        evt.NewCategoryId.Should().Be(ProductData.NewCategoryId);
    }

    #endregion

    #region AddFeature

    [Fact]
    public void AddFeature_Should_ReturnFailure_WhenFeatureIdIsEmpty()
    {
        // Arrange
        var product = ProductData.CreateProduct();

        // Act
        var result = product.AddFeature(Guid.Empty, ProductData.FeatureName, ProductData.FeatureValue, ProductData.FeatureDisplayOrder);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.InvalidFeatureId);
    }

    [Fact]
    public void AddFeature_Should_ReturnFailure_WhenNameIsEmpty()
    {
        // Arrange
        var product = ProductData.CreateProduct();

        // Act
        var result = product.AddFeature(ProductData.FeatureId, "", ProductData.FeatureValue, ProductData.FeatureDisplayOrder);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.InvalidFeatureName);
    }

    [Fact]
    public void AddFeature_Should_ReturnFailure_WhenValueIsEmpty()
    {
        // Arrange
        var product = ProductData.CreateProduct();

        // Act
        var result = product.AddFeature(ProductData.FeatureId, ProductData.FeatureName, "", ProductData.FeatureDisplayOrder);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.InvalidFeatureValue);
    }

    [Fact]
    public void AddFeature_Should_ReturnFailure_WhenProductIsDiscontinued()
    {
        // Arrange
        var product = ProductData.CreateDiscontinuedProduct();

        // Act
        var result = product.AddFeature(ProductData.FeatureId, ProductData.FeatureName, ProductData.FeatureValue, ProductData.FeatureDisplayOrder);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.DiscontinuedCannotBeModified);
    }

    [Fact]
    public void AddFeature_Should_ReturnFailure_WhenFeatureWithSameIdAlreadyExists()
    {
        // Arrange
        var product = ProductData.CreateProductWithFeature();

        // Act
        var result = product.AddFeature(ProductData.FeatureId, "Different Name", ProductData.FeatureValue, ProductData.FeatureDisplayOrder);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.DuplicateFeatureId);
    }

    [Fact]
    public void AddFeature_Should_ReturnFailure_WhenFeatureWithSameNameAlreadyExists()
    {
        // Arrange
        var product = ProductData.CreateProductWithFeature();

        // Act
        var result = product.AddFeature(Guid.NewGuid(), ProductData.FeatureName, ProductData.FeatureValue, ProductData.FeatureDisplayOrder);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.FeatureAlreadyExists);
    }

    [Fact]
    public void AddFeature_Should_ReturnSuccess_WhenParametersAreValid()
    {
        // Arrange
        var product = ProductData.CreateProduct();

        // Act
        var result = product.AddFeature(ProductData.FeatureId, ProductData.FeatureName, ProductData.FeatureValue, ProductData.FeatureDisplayOrder);

        // Assert
        result.IsSuccess.Should().BeTrue();
        product.Features.Should().ContainSingle(f => f.Id == ProductData.FeatureId && f.Name == ProductData.FeatureName);
    }

    [Fact]
    public void AddFeature_Should_RaiseDomainEvent_WhenFeatureIsAdded()
    {
        // Arrange
        var product = ProductData.CreateProduct();

        // Act
        product.AddFeature(ProductData.FeatureId, ProductData.FeatureName, ProductData.FeatureValue, ProductData.FeatureDisplayOrder);

        // Assert
        var evt = product.GetDomainEvents().OfType<ProductFeatureAddedDomainEvent>().SingleOrDefault();
        evt.Should().NotBeNull();
        evt!.ProductId.Should().Be(product.Id);
        evt.FeatureId.Should().Be(ProductData.FeatureId);
        evt.Name.Should().Be(ProductData.FeatureName);
        evt.Value.Should().Be(ProductData.FeatureValue);
    }

    #endregion

    #region UpdateFeatureValue

    [Fact]
    public void UpdateFeatureValue_Should_ReturnFailure_WhenFeatureIdIsEmpty()
    {
        // Arrange
        var product = ProductData.CreateProductWithFeature();

        // Act
        var result = product.UpdateFeatureValue(Guid.Empty, ProductData.NewFeatureValue);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.InvalidFeatureId);
    }

    [Fact]
    public void UpdateFeatureValue_Should_ReturnFailure_WhenValueIsEmpty()
    {
        // Arrange
        var product = ProductData.CreateProductWithFeature();

        // Act
        var result = product.UpdateFeatureValue(ProductData.FeatureId, "");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.InvalidFeatureValue);
    }

    [Fact]
    public void UpdateFeatureValue_Should_ReturnFailure_WhenProductIsDiscontinued()
    {
        // Arrange
        var product = ProductData.CreateDiscontinuedProduct();

        // Act
        var result = product.UpdateFeatureValue(ProductData.FeatureId, ProductData.NewFeatureValue);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.DiscontinuedCannotBeModified);
    }

    [Fact]
    public void UpdateFeatureValue_Should_ReturnFailure_WhenFeatureIsNotFound()
    {
        // Arrange
        var product = ProductData.CreateProduct();

        // Act
        var result = product.UpdateFeatureValue(ProductData.FeatureId, ProductData.NewFeatureValue);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.FeatureNotFound);
    }

    [Fact]
    public void UpdateFeatureValue_Should_ReturnFailure_WhenValueIsUnchanged()
    {
        // Arrange
        var product = ProductData.CreateProductWithFeature();

        // Act
        var result = product.UpdateFeatureValue(ProductData.FeatureId, ProductData.FeatureValue);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.FeatureValueUnchanged);
    }

    [Fact]
    public void UpdateFeatureValue_Should_ReturnSuccess_WhenValueIsValid()
    {
        // Arrange
        var product = ProductData.CreateProductWithFeature();

        // Act
        var result = product.UpdateFeatureValue(ProductData.FeatureId, ProductData.NewFeatureValue);

        // Assert
        result.IsSuccess.Should().BeTrue();
        product.Features.Single(f => f.Id == ProductData.FeatureId).Value.Should().Be(ProductData.NewFeatureValue);
    }

    [Fact]
    public void UpdateFeatureValue_Should_RaiseDomainEvent_WhenFeatureValueIsUpdated()
    {
        // Arrange
        var product = ProductData.CreateProductWithFeature();

        // Act
        product.UpdateFeatureValue(ProductData.FeatureId, ProductData.NewFeatureValue);

        // Assert
        var evt = product.GetDomainEvents().OfType<ProductFeatureUpdatedDomainEvent>().SingleOrDefault();
        evt.Should().NotBeNull();
        evt!.ProductId.Should().Be(product.Id);
        evt.FeatureId.Should().Be(ProductData.FeatureId);
        evt.OldValue.Should().Be(ProductData.FeatureValue);
        evt.NewValue.Should().Be(ProductData.NewFeatureValue);
    }

    #endregion

    #region RemoveFeature

    [Fact]
    public void RemoveFeature_Should_ReturnFailure_WhenFeatureIdIsEmpty()
    {
        // Arrange
        var product = ProductData.CreateProductWithFeature();

        // Act
        var result = product.RemoveFeature(Guid.Empty);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.InvalidFeatureId);
    }

    [Fact]
    public void RemoveFeature_Should_ReturnFailure_WhenProductIsDiscontinued()
    {
        // Arrange
        var product = ProductData.CreateDiscontinuedProduct();

        // Act
        var result = product.RemoveFeature(ProductData.FeatureId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.DiscontinuedCannotBeModified);
    }

    [Fact]
    public void RemoveFeature_Should_ReturnFailure_WhenFeatureIsNotFound()
    {
        // Arrange
        var product = ProductData.CreateProduct();

        // Act
        var result = product.RemoveFeature(ProductData.FeatureId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.FeatureNotFound);
    }

    [Fact]
    public void RemoveFeature_Should_ReturnSuccess_WhenFeatureExists()
    {
        // Arrange
        var product = ProductData.CreateProductWithFeature();

        // Act
        var result = product.RemoveFeature(ProductData.FeatureId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        product.Features.Should().BeEmpty();
    }

    [Fact]
    public void RemoveFeature_Should_RaiseDomainEvent_WhenFeatureIsRemoved()
    {
        // Arrange
        var product = ProductData.CreateProductWithFeature();

        // Act
        product.RemoveFeature(ProductData.FeatureId);

        // Assert
        var evt = product.GetDomainEvents().OfType<ProductFeatureRemovedDomainEvent>().SingleOrDefault();
        evt.Should().NotBeNull();
        evt!.ProductId.Should().Be(product.Id);
        evt.FeatureId.Should().Be(ProductData.FeatureId);
        evt.Name.Should().Be(ProductData.FeatureName);
    }

    #endregion
}
