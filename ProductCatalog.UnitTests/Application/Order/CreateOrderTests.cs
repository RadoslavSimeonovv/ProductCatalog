using FluentAssertions;
using NSubstitute;
using ProductCatalog.Application.Abstractions.Authentication;
using ProductCatalog.Application.Order;
using ProductCatalog.Application.Order.CreateOrder;
using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Order.Errors;
using ProductCatalog.Domain.Order.Repositories;
using OrderEntity = ProductCatalog.Domain.Order.Entities.Order;

namespace ProductCatalog.UnitTests.Application.Order;

public class CreateOrderTests
{
    private static readonly List<CreateOrderItemDto> ValidItems =
    [
        new CreateOrderItemDto(Guid.NewGuid(), 2, 9.99m, "USD")
    ];

    private static readonly CreateOrderCommand ValidCommand = new(Items: ValidItems);

    private readonly CreateOrderCommandHandler _handler;
    private readonly IOrderRepository _orderRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly ICurrentUser _currentUserMock;

    public CreateOrderTests()
    {
        _orderRepositoryMock = Substitute.For<IOrderRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _currentUserMock = Substitute.For<ICurrentUser>();

        _currentUserMock.IsAuthenticated.Returns(true);
        _currentUserMock.UserId.Returns("user-123");
        _currentUserMock.Email.Returns("customer@test.com");

        _handler = new CreateOrderCommandHandler(_orderRepositoryMock, _unitOfWorkMock, _currentUserMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_User_Not_Authenticated()
    {
        // Arrange
        _currentUserMock.IsAuthenticated.Returns(false);

        // Act
        var result = await _handler.Handle(ValidCommand, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.Unauthorized);
        _orderRepositoryMock.DidNotReceive().Add(Arg.Any<OrderEntity>());
        await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_UserId_Is_Null()
    {
        // Arrange
        _currentUserMock.UserId.Returns((string?)null);

        // Act
        var result = await _handler.Handle(ValidCommand, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.Unauthorized);
        _orderRepositoryMock.DidNotReceive().Add(Arg.Any<OrderEntity>());
        await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_Email_Is_Null()
    {
        // Arrange
        _currentUserMock.Email.Returns((string?)null);

        // Act
        var result = await _handler.Handle(ValidCommand, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.Unauthorized);
        _orderRepositoryMock.DidNotReceive().Add(Arg.Any<OrderEntity>());
        await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_Items_Are_Empty()
    {
        // Arrange
        var command = ValidCommand with { Items = [] };

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.EmptyOrder);
        _orderRepositoryMock.DidNotReceive().Add(Arg.Any<OrderEntity>());
        await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_Currency_Is_Invalid()
    {
        // Arrange
        var command = new CreateOrderCommand(
        [
            new CreateOrderItemDto(Guid.NewGuid(), 1, 10.00m, "INVALID")
        ]);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        _orderRepositoryMock.DidNotReceive().Add(Arg.Any<OrderEntity>());
        await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_Items_Have_Currency_Mismatch()
    {
        // Arrange
        var command = new CreateOrderCommand(
        [
            new CreateOrderItemDto(Guid.NewGuid(), 1, 10.00m, "USD"),
            new CreateOrderItemDto(Guid.NewGuid(), 1, 12.00m, "EUR")
        ]);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.CurrencyMismatch);
        _orderRepositoryMock.DidNotReceive().Add(Arg.Any<OrderEntity>());
        await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_And_PersistOrder_When_Command_Is_Valid()
    {
        // Arrange (defaults set in constructor)

        // Act
        var result = await _handler.Handle(ValidCommand, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        _orderRepositoryMock.Received(1).Add(Arg.Any<OrderEntity>());
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
