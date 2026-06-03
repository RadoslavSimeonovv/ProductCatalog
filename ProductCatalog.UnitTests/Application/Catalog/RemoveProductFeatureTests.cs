using FluentAssertions;
using NSubstitute;
using ProductCatalog.Application.Catalog.RemoveProductFeature;
using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Catalog.Entities;
using ProductCatalog.Domain.Catalog.Errors;
using ProductCatalog.Domain.Catalog.Repositories;
using ProductCatalog.UnitTests.Domain.Catalog;

namespace ProductCatalog.UnitTests.Application.Catalog;

public class RemoveProductFeatureTests
{
    private static readonly Guid ProductId = Guid.NewGuid();

    private readonly RemoveProductFeatureCommandHandler _handler;
    private readonly IProductRepository _productRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;

    public RemoveProductFeatureTests()
    {
        _productRepositoryMock = Substitute.For<IProductRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();

        _handler = new RemoveProductFeatureCommandHandler(_productRepositoryMock, _unitOfWorkMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_Product_Not_Found()
    {
        // Arrange
        _productRepositoryMock
            .GetByIdAsync(ProductId, Arg.Any<CancellationToken>())
            .Returns((Product?)null);

        var command = new RemoveProductFeatureCommand(ProductId, ProductData.FeatureId);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.NotFound);
        await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_Product_Is_Discontinued()
    {
        // Arrange
        _productRepositoryMock
            .GetByIdAsync(ProductId, Arg.Any<CancellationToken>())
            .Returns(ProductData.CreateDiscontinuedProduct());

        var command = new RemoveProductFeatureCommand(ProductId, ProductData.FeatureId);

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

        var command = new RemoveProductFeatureCommand(ProductId, ProductData.FeatureId);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.FeatureNotFound);
        await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_Feature_Exists()
    {
        // Arrange
        _productRepositoryMock
            .GetByIdAsync(ProductId, Arg.Any<CancellationToken>())
            .Returns(ProductData.CreateProductWithFeature());

        var command = new RemoveProductFeatureCommand(ProductId, ProductData.FeatureId);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
