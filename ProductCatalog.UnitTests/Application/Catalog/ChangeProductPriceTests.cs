using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using ProductCatalog.Application.Catalog.ChangeProductPrice;
using ProductCatalog.Application.Exceptions;
using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Catalog.Entities;
using ProductCatalog.Domain.Catalog.Errors;
using ProductCatalog.Domain.Catalog.Repositories;
using ProductCatalog.Domain.Shared.Errors;
using ProductCatalog.UnitTests.Domain.Catalog;

namespace ProductCatalog.UnitTests.Application.Catalog;

public class ChangeProductPriceTests
{
    private static readonly Guid ProductId = Guid.NewGuid();

    private readonly ChangeProductPriceCommandHandler _handler;
    private readonly IProductRepository _productRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;

    public ChangeProductPriceTests()
    {
        _productRepositoryMock = Substitute.For<IProductRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();

        _handler = new ChangeProductPriceCommandHandler(_productRepositoryMock, _unitOfWorkMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_Product_Not_Found()
    {
        // Arrange
        _productRepositoryMock
            .GetByIdAsync(ProductId, Arg.Any<CancellationToken>())
            .Returns((Product?)null);

        var command = new ChangeProductPriceCommand(ProductId, 19.99m, "USD");

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.NotFound);
        await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_Currency_Is_Invalid()
    {
        // Arrange
        _productRepositoryMock
            .GetByIdAsync(ProductId, Arg.Any<CancellationToken>())
            .Returns(ProductData.CreateProduct());

        var command = new ChangeProductPriceCommand(ProductId, 19.99m, "XYZ");

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(CurrencyErrors.Unsupported("XYZ").Code);
        await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_Price_Is_Unchanged()
    {
        // Arrange — ProductData.Price is 9.99 USD
        _productRepositoryMock
            .GetByIdAsync(ProductId, Arg.Any<CancellationToken>())
            .Returns(ProductData.CreateProduct());

        var command = new ChangeProductPriceCommand(ProductId, 9.99m, "USD");

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.PriceUnchanged);
        await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ConcurrencyException_Is_Thrown()
    {
        // Arrange
        _productRepositoryMock
            .GetByIdAsync(ProductId, Arg.Any<CancellationToken>())
            .Returns(ProductData.CreateProduct());

        _unitOfWorkMock
            .SaveChangesAsync(Arg.Any<CancellationToken>())
            .ThrowsAsync(new ConcurrencyException("conflict", null!));

        var command = new ChangeProductPriceCommand(ProductId, 19.99m, "USD");

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.ConcurrencyConflict);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_Command_Is_Valid()
    {
        // Arrange
        _productRepositoryMock
            .GetByIdAsync(ProductId, Arg.Any<CancellationToken>())
            .Returns(ProductData.CreateProduct());

        var command = new ChangeProductPriceCommand(ProductId, 19.99m, "USD");

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
