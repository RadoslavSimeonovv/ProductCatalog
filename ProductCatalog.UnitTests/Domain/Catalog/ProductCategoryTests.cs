using FluentAssertions;
using ProductCatalog.Domain.Catalog.Entities;
using ProductCatalog.Domain.Catalog.Errors;

namespace ProductCatalog.UnitTests.Domain.Catalog;

public class ProductCategoryTests
{
    [Fact]
    public void Create_ShouldReturnFailure_WhenNameIsInvalid()
    { 
        // Act
        var result = ProductCategory.Create(ProductCategoryData.InvalidName, ProductCategoryData.Description);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.InvalidCategoryName);
    }

    [Fact]
    public void Create_Should_ReturnSuccess_WhenNameAndDescriptionAreValid()
    {
        // Act
        var result = ProductCategory.Create(ProductCategoryData.Name, ProductCategoryData.Description);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be(ProductCategoryData.Name);
        result.Value.Description.Should().Be(ProductCategoryData.Description);
    }


    [Fact]
    public void UpdateDetails_Should_ReturnFailure_WhenNameIsInvalid()
    {
        // Arrange
        var category = ProductCategory.Create(ProductCategoryData.Name, ProductCategoryData.Description).Value;

        // Act
        var result = category.UpdateDetails(ProductCategoryData.InvalidName, ProductCategoryData.Description);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.InvalidCategoryName);
    }

    [Fact]
    public void UpdateDetails_Should_ReturnSuccess_WhenNameAndDescriptionAreValid()
    {
        // Arrange
        var category = ProductCategory.Create(ProductCategoryData.Name, ProductCategoryData.Description).Value;
        var newName = "Updated Category";
        var newDescription = "Updated Description";

        // Act
        var result = category.UpdateDetails(newName, newDescription);

        // Assert
        result.IsSuccess.Should().BeTrue();
        category.Name.Should().Be(newName);
        category.Description.Should().Be(newDescription);
    }
}