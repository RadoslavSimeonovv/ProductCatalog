using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using ProductCatalog.Application.Exceptions;
using ProductCatalog.Application.Order.SubmitOrderForPayment;
using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Order.Errors;
using ProductCatalog.Domain.Order.Repositories;
using ProductCatalog.Domain.Payment.Repositories;
using ProductCatalog.UnitTests.Domain.Order;
using ProductCatalog.UnitTests.Domain.Payment;
using OrderEntity = ProductCatalog.Domain.Order.Entities.Order;
using PaymentEntity = ProductCatalog.Domain.Payment.Entities.Payment;

namespace ProductCatalog.UnitTests.Application.Order;

public class SubmitOrderForPaymentTests
{
    private static readonly Guid OrderId = OrderData.OrderId;
    private const string Provider = "Stripe";
    private const string IdempotencyKey = "idem-key-001";

    private static readonly SubmitOrderForPaymentCommand ValidCommand = new(
        OrderId: OrderId,
        Provider: Provider,
        IdempotencyKey: IdempotencyKey);

    private readonly SubmitOrderForPaymentCommandHandler _handler;
    private readonly IOrderRepository _orderRepositoryMock;
    private readonly IPaymentRepository _paymentRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;

    public SubmitOrderForPaymentTests()
    {
        _orderRepositoryMock = Substitute.For<IOrderRepository>();
        _paymentRepositoryMock = Substitute.For<IPaymentRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();

        _paymentRepositoryMock
            .GetByIdempotencyKeyAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((PaymentEntity?)null);

        _handler = new SubmitOrderForPaymentCommandHandler(
            _orderRepositoryMock,
            _paymentRepositoryMock,
            _unitOfWorkMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnExistingPaymentId_When_IdempotencyKey_Already_Exists()
    {
        // Arrange
        var existingPayment = PaymentData.CreatePayment();

        _paymentRepositoryMock
            .GetByIdempotencyKeyAsync(IdempotencyKey, Provider, Arg.Any<CancellationToken>())
            .Returns(existingPayment);

        // Act
        var result = await _handler.Handle(ValidCommand, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(existingPayment.Id);
        await _orderRepositoryMock.DidNotReceive().GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
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
        _paymentRepositoryMock.DidNotReceive().Add(Arg.Any<PaymentEntity>());
        await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_Order_Is_Not_In_Created_State()
    {
        // Arrange
        _orderRepositoryMock
            .GetByIdAsync(OrderId, Arg.Any<CancellationToken>())
            .Returns(OrderData.CreateSubmittedOrder());

        // Act
        var result = await _handler.Handle(ValidCommand, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.NotCreated);
        _paymentRepositoryMock.DidNotReceive().Add(Arg.Any<PaymentEntity>());
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
    public async Task Handle_Should_ReturnSuccess_And_CreatePayment_When_Order_Is_Valid()
    {
        // Arrange
        _orderRepositoryMock
            .GetByIdAsync(OrderId, Arg.Any<CancellationToken>())
            .Returns(OrderData.CreateOrder());

        // Act
        var result = await _handler.Handle(ValidCommand, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        _paymentRepositoryMock.Received(1).Add(Arg.Any<PaymentEntity>());
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
