using FluentAssertions;
using NSubstitute;
using ProductCatalog.Application.Catalog.ChangeProductCategory;
using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Catalog.Entities;
using ProductCatalog.Domain.Catalog.Errors;
using ProductCatalog.Domain.Catalog.Repositories;
using ProductCatalog.UnitTests.Domain.Catalog;

namespace ProductCatalog.UnitTests.Application.Catalog;

public class ChangeProductCategoryTests
{
    private static readonly Guid ProductId = Guid.NewGuid();
    private static readonly Guid NewCategoryId = Guid.NewGuid();

    private readonly ChangeProductCategoryCommandHandler _handler;
    private readonly IProductRepository _productRepositoryMock;
    private readonly IProductCategoryRepository _productCategoryRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;

    public ChangeProductCategoryTests()
    {
        _productRepositoryMock = Substitute.For<IProductRepository>();
        _productCategoryRepositoryMock = Substitute.For<IProductCategoryRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();

        _handler = new ChangeProductCategoryCommandHandler(
            _productRepositoryMock,
            _productCategoryRepositoryMock,
            _unitOfWorkMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_Product_Not_Found()
    {
        // Arrange
        _productRepositoryMock
            .GetByIdAsync(ProductId, Arg.Any<CancellationToken>())
            .Returns((Product?)null);

        var command = new ChangeProductCategoryCommand(ProductId, NewCategoryId);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.NotFound);
        await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_Category_Not_Found()
    {
        // Arrange
        _productRepositoryMock
            .GetByIdAsync(ProductId, Arg.Any<CancellationToken>())
            .Returns(ProductData.CreateProduct());

        _productCategoryRepositoryMock
            .GetByIdAsync(NewCategoryId, Arg.Any<CancellationToken>())
            .Returns((ProductCategory?)null);

        var command = new ChangeProductCategoryCommand(ProductId, NewCategoryId);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.CategoryNotFound);
        await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_Category_Is_Unchanged()
    {
        // Arrange — use the same CategoryId the product already has
        _productRepositoryMock
            .GetByIdAsync(ProductId, Arg.Any<CancellationToken>())
            .Returns(ProductData.CreateProduct());

        _productCategoryRepositoryMock
            .GetByIdAsync(ProductData.CategoryId, Arg.Any<CancellationToken>())
            .Returns(ProductCategoryData.CreateCategory());

        var command = new ChangeProductCategoryCommand(ProductId, ProductData.CategoryId);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.CategoryUnchanged);
        await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_Product_Is_Discontinued()
    {
        // Arrange
        _productRepositoryMock
            .GetByIdAsync(ProductId, Arg.Any<CancellationToken>())
            .Returns(ProductData.CreateDiscontinuedProduct());

        _productCategoryRepositoryMock
            .GetByIdAsync(NewCategoryId, Arg.Any<CancellationToken>())
            .Returns(ProductCategoryData.CreateCategory());

        var command = new ChangeProductCategoryCommand(ProductId, NewCategoryId);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.DiscontinuedCannotBeModified);
        await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_Command_Is_Valid()
    {
        // Arrange
        _productRepositoryMock
            .GetByIdAsync(ProductId, Arg.Any<CancellationToken>())
            .Returns(ProductData.CreateProduct());

        _productCategoryRepositoryMock
            .GetByIdAsync(NewCategoryId, Arg.Any<CancellationToken>())
            .Returns(ProductCategoryData.CreateCategory());

        var command = new ChangeProductCategoryCommand(ProductId, NewCategoryId);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
