using FluentAssertions;
using NSubstitute;
using ProductCatalog.Application.Catalog.CreateProduct;
using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Catalog.Entities;
using ProductCatalog.Domain.Catalog.Errors;
using ProductCatalog.Domain.Catalog.Repositories;
using ProductCatalog.Domain.Shared.Errors;
using ProductCatalog.UnitTests.Domain.Catalog;

namespace ProductCatalog.UnitTests.Application.Catalog;

public class CreateProductTests
{
    private static readonly Guid CategoryId = Guid.NewGuid();

    private static readonly CreateProductCommand ValidCommand = new(
        Name: "Test Product",
        Description: "Test Description",
        Sku: "SKU-001",
        CategoryId: CategoryId,
        PriceAmount: 9.99m,
        Currency: "USD");

    private readonly CreateProductCommandHandler _handler;
    private readonly IProductRepository _productRepositoryMock;
    private readonly IProductCategoryRepository _productCategoryRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;

    public CreateProductTests()
    {
        _productRepositoryMock = Substitute.For<IProductRepository>();
        _productCategoryRepositoryMock = Substitute.For<IProductCategoryRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();

        _handler = new CreateProductCommandHandler(
            _productRepositoryMock,
            _productCategoryRepositoryMock,
            _unitOfWorkMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_Category_Not_Found()
    {
        // Arrange
        _productCategoryRepositoryMock
            .GetByIdAsync(CategoryId, Arg.Any<CancellationToken>())
            .Returns((ProductCategory?)null);

        // Act
        var result = await _handler.Handle(ValidCommand, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.CategoryNotFound);
        _productRepositoryMock.DidNotReceive().Add(Arg.Any<Product>());
        await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_Sku_Is_Invalid()
    {
        // Arrange
        _productCategoryRepositoryMock
            .GetByIdAsync(CategoryId, Arg.Any<CancellationToken>())
            .Returns(ProductCategoryData.CreateCategory());

        var command = ValidCommand with { Sku = "INVALID SKU!" };

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.InvalidSku);
        _productRepositoryMock.DidNotReceive().Add(Arg.Any<Product>());
        await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_Currency_Is_Invalid()
    {
        // Arrange
        _productCategoryRepositoryMock
            .GetByIdAsync(CategoryId, Arg.Any<CancellationToken>())
            .Returns(ProductCategoryData.CreateCategory());

        var command = ValidCommand with { Currency = "XYZ" };

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(CurrencyErrors.Unsupported("XYZ").Code);
        _productRepositoryMock.DidNotReceive().Add(Arg.Any<Product>());
        await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_Product_Name_Is_Invalid()
    {
        // Arrange
        _productCategoryRepositoryMock
            .GetByIdAsync(CategoryId, Arg.Any<CancellationToken>())
            .Returns(ProductCategoryData.CreateCategory());

        var command = ValidCommand with { Name = "" };

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.InvalidName);
        _productRepositoryMock.DidNotReceive().Add(Arg.Any<Product>());
        await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_And_PersistProduct_When_Command_Is_Valid()
    {
        // Arrange
        _productCategoryRepositoryMock
            .GetByIdAsync(CategoryId, Arg.Any<CancellationToken>())
            .Returns(ProductCategoryData.CreateCategory());

        // Act
        var result = await _handler.Handle(ValidCommand, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        _productRepositoryMock.Received(1).Add(Arg.Any<Product>());
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
