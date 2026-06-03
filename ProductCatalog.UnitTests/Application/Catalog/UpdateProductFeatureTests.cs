using FluentAssertions;
using NSubstitute;
using ProductCatalog.Application.Catalog.UpdateProductFeature;
using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Catalog.Entities;
using ProductCatalog.Domain.Catalog.Errors;
using ProductCatalog.Domain.Catalog.Repositories;
using ProductCatalog.UnitTests.Domain.Catalog;

namespace ProductCatalog.UnitTests.Application.Catalog;

public class UpdateProductFeatureTests
{
    private static readonly Guid ProductId = Guid.NewGuid();

    private readonly UpdateProductFeatureValueCommandHandler _handler;
    private readonly IProductRepository _productRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;

    public UpdateProductFeatureTests()
    {
        _productRepositoryMock = Substitute.For<IProductRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();

        _handler = new UpdateProductFeatureValueCommandHandler(_productRepositoryMock, _unitOfWorkMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_Product_Not_Found()
    {
        // Arrange
        _productRepositoryMock
            .GetByIdAsync(ProductId, Arg.Any<CancellationToken>())
            .Returns((Product?)null);

        var command = new UpdateProductFeatureValueCommand(ProductId, ProductData.FeatureId, "Blue");

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.NotFound);
        await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_New_Value_Is_Invalid()
    {
        // Arrange
        _productRepositoryMock
            .GetByIdAsync(ProductId, Arg.Any<CancellationToken>())
            .Returns(ProductData.CreateProductWithFeature());

        var command = new UpdateProductFeatureValueCommand(ProductId, ProductData.FeatureId, "");

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.InvalidFeatureValue);
        await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_Product_Is_Discontinued()
    {
        // Arrange
        _productRepositoryMock
            .GetByIdAsync(ProductId, Arg.Any<CancellationToken>())
            .Returns(ProductData.CreateDiscontinuedProduct());

        var command = new UpdateProductFeatureValueCommand(ProductId, ProductData.FeatureId, "Blue");

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.DiscontinuedCannotBeModified);
        await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_Feature_Not_Found()
    {
        // Arrange — product exists but has no features
        _productRepositoryMock
            .GetByIdAsync(ProductId, Arg.Any<CancellationToken>())
            .Returns(ProductData.CreateProduct());

        var command = new UpdateProductFeatureValueCommand(ProductId, ProductData.FeatureId, "Blue");

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.FeatureNotFound);
        await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_Value_Is_Unchanged()
    {
        // Arrange — current value is ProductData.FeatureValue = "Red"
        _productRepositoryMock
            .GetByIdAsync(ProductId, Arg.Any<CancellationToken>())
            .Returns(ProductData.CreateProductWithFeature());

        var command = new UpdateProductFeatureValueCommand(ProductId, ProductData.FeatureId, ProductData.FeatureValue);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.FeatureValueUnchanged);
        await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_Command_Is_Valid()
    {
        // Arrange
        _productRepositoryMock
            .GetByIdAsync(ProductId, Arg.Any<CancellationToken>())
            .Returns(ProductData.CreateProductWithFeature());

        var command = new UpdateProductFeatureValueCommand(ProductId, ProductData.FeatureId, ProductData.NewFeatureValue);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
