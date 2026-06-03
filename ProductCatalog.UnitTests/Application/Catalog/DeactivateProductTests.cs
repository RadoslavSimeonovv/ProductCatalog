using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using ProductCatalog.Application.Catalog.DeactivateProduct;
using ProductCatalog.Application.Exceptions;
using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Catalog.Entities;
using ProductCatalog.Domain.Catalog.Errors;
using ProductCatalog.Domain.Catalog.Repositories;
using ProductCatalog.UnitTests.Domain.Catalog;

namespace ProductCatalog.UnitTests.Application.Catalog;

public class DeactivateProductTests
{
    private static readonly Guid ProductId = Guid.NewGuid();

    private readonly DeactivateProductCommandHandler _handler;
    private readonly IProductRepository _productRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;

    public DeactivateProductTests()
    {
        _productRepositoryMock = Substitute.For<IProductRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();

        _handler = new DeactivateProductCommandHandler(_productRepositoryMock, _unitOfWorkMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_Product_Not_Found()
    {
        // Arrange
        _productRepositoryMock
            .GetByIdAsync(ProductId, Arg.Any<CancellationToken>())
            .Returns((Product?)null);

        // Act
        var result = await _handler.Handle(new DeactivateProductCommand(ProductId), default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.NotFound);
        await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_Product_Is_Not_Active()
    {
        // Arrange — Draft product cannot be deactivated
        _productRepositoryMock
            .GetByIdAsync(ProductId, Arg.Any<CancellationToken>())
            .Returns(ProductData.CreateProduct());

        // Act
        var result = await _handler.Handle(new DeactivateProductCommand(ProductId), default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.NotActive);
        await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ConcurrencyException_Is_Thrown()
    {
        // Arrange
        _productRepositoryMock
            .GetByIdAsync(ProductId, Arg.Any<CancellationToken>())
            .Returns(ProductData.CreateActiveProduct());

        _unitOfWorkMock
            .SaveChangesAsync(Arg.Any<CancellationToken>())
            .ThrowsAsync(new ConcurrencyException("conflict", null!));

        // Act
        var result = await _handler.Handle(new DeactivateProductCommand(ProductId), default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.ConcurrencyConflict);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_Product_Is_Active()
    {
        // Arrange
        _productRepositoryMock
            .GetByIdAsync(ProductId, Arg.Any<CancellationToken>())
            .Returns(ProductData.CreateActiveProduct());

        // Act
        var result = await _handler.Handle(new DeactivateProductCommand(ProductId), default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
