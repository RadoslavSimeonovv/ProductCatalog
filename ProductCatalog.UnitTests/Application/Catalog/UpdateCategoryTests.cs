using FluentAssertions;
using NSubstitute;
using ProductCatalog.Application.Catalog.UpdateCategory;
using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Catalog.Entities;
using ProductCatalog.Domain.Catalog.Errors;
using ProductCatalog.Domain.Catalog.Repositories;
using ProductCatalog.UnitTests.Domain.Catalog;

namespace ProductCatalog.UnitTests.Application.Catalog;

public class UpdateCategoryTests
{
    private static readonly Guid CategoryId = Guid.NewGuid();

    private readonly UpdateCategoryCommandHandler _handler;
    private readonly IProductCategoryRepository _productCategoryRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;

    public UpdateCategoryTests()
    {
        _productCategoryRepositoryMock = Substitute.For<IProductCategoryRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();

        _handler = new UpdateCategoryCommandHandler(_productCategoryRepositoryMock, _unitOfWorkMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_Category_Not_Found()
    {
        // Arrange
        _productCategoryRepositoryMock
            .GetByIdAsync(CategoryId, Arg.Any<CancellationToken>())
            .Returns((ProductCategory?)null);

        var command = new UpdateCategoryCommand(CategoryId, "New Name", "New Description");

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.CategoryNotFound);
        await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_Name_Is_Invalid()
    {
        // Arrange
        _productCategoryRepositoryMock
            .GetByIdAsync(CategoryId, Arg.Any<CancellationToken>())
            .Returns(ProductCategoryData.CreateCategory());

        var command = new UpdateCategoryCommand(CategoryId, "", "New Description");

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.InvalidCategoryName);
        await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_Command_Is_Valid()
    {
        // Arrange
        _productCategoryRepositoryMock
            .GetByIdAsync(CategoryId, Arg.Any<CancellationToken>())
            .Returns(ProductCategoryData.CreateCategory());

        var command = new UpdateCategoryCommand(CategoryId, "Updated Name", "Updated Description");

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
