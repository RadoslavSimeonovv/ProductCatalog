using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using ProductCatalog.Application.Abstractions.Authentication;
using ProductCatalog.Application.Exceptions;
using ProductCatalog.Application.Order.CancelOrder;
using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Order.Errors;
using ProductCatalog.Domain.Order.Repositories;
using ProductCatalog.UnitTests.Domain.Order;
using OrderEntity = ProductCatalog.Domain.Order.Entities.Order;

namespace ProductCatalog.UnitTests.Application.Order;

public class CancelOrderTests
{
    private static readonly Guid OrderId = OrderData.OrderId;

    private static readonly CancelOrderCommand ValidCommand = new(
        OrderId: OrderId,
        Reason: "No longer needed");

    private readonly CancelOrderCommandHandler _handler;
    private readonly IOrderRepository _orderRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly ICurrentUser _currentUserMock;

    public CancelOrderTests()
    {
        _orderRepositoryMock = Substitute.For<IOrderRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _currentUserMock = Substitute.For<ICurrentUser>();

        // Default: authenticated customer who owns the order
        _currentUserMock.UserId.Returns(OrderData.CustomerId.Value);
        _currentUserMock.IsInRole(Roles.Admin).Returns(false);

        _handler = new CancelOrderCommandHandler(_orderRepositoryMock, _unitOfWorkMock, _currentUserMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_Order_Not_Found()
    {
        // Arrange
        _orderRepositoryMock
            .GetByIdAsync(OrderId, Arg.Any<CancellationToken>())
            .Returns((OrderEntity?)null);

        // Act
        var result = await _handler.Handle(ValidCommand, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.NotFound);
        await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_User_Is_Not_Owner_Or_Admin()
    {
        // Arrange
        _orderRepositoryMock
            .GetByIdAsync(OrderId, Arg.Any<CancellationToken>())
            .Returns(OrderData.CreateOrder());

        _currentUserMock.UserId.Returns("different-user");

        // Act
        var result = await _handler.Handle(ValidCommand, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.Unauthorized);
        await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_Order_Is_Already_Cancelled()
    {
        // Arrange
        _orderRepositoryMock
            .GetByIdAsync(OrderId, Arg.Any<CancellationToken>())
            .Returns(OrderData.CreateCancelledOrder());

        // Act
        var result = await _handler.Handle(ValidCommand, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.AlreadyCancelled);
        await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_Order_Is_Paid()
    {
        // Arrange
        _orderRepositoryMock
            .GetByIdAsync(OrderId, Arg.Any<CancellationToken>())
            .Returns(OrderData.CreatePaidOrder());

        // Act
        var result = await _handler.Handle(ValidCommand, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.CannotCancelPaidOrder);
        await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ConcurrencyException_Is_Thrown()
    {
        // Arrange
        _orderRepositoryMock
            .GetByIdAsync(OrderId, Arg.Any<CancellationToken>())
            .Returns(OrderData.CreateOrder());

        _unitOfWorkMock
            .SaveChangesAsync(Arg.Any<CancellationToken>())
            .ThrowsAsync(new ConcurrencyException("conflict", null!));

        // Act
        var result = await _handler.Handle(ValidCommand, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.ConcurrencyConflict);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_Customer_Cancels_Own_Order()
    {
        // Arrange
        _orderRepositoryMock
            .GetByIdAsync(OrderId, Arg.Any<CancellationToken>())
            .Returns(OrderData.CreateOrder());

        // Act
        var result = await _handler.Handle(ValidCommand, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_Admin_Cancels_Any_Order()
    {
        // Arrange
        _orderRepositoryMock
            .GetByIdAsync(OrderId, Arg.Any<CancellationToken>())
            .Returns(OrderData.CreateOrder());

        _currentUserMock.UserId.Returns("different-user");
        _currentUserMock.IsInRole(Roles.Admin).Returns(true);

        // Act
        var result = await _handler.Handle(ValidCommand, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
