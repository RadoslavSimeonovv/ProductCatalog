using FluentAssertions;
using NSubstitute;
using ProductCatalog.Application.Payment.CreatePayment;
using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Order.Errors;
using ProductCatalog.Domain.Order.Repositories;
using ProductCatalog.Domain.Payment.Errors;
using ProductCatalog.Domain.Payment.Repositories;
using ProductCatalog.UnitTests.Domain.Order;
using OrderEntity = ProductCatalog.Domain.Order.Entities.Order;
using PaymentEntity = ProductCatalog.Domain.Payment.Entities.Payment;

namespace ProductCatalog.UnitTests.Application.Payment;

public class CreatePaymentTests
{
    private static readonly Guid OrderId = OrderData.OrderId;
    private const string Provider = "Stripe";
    private const string IdempotencyKey = "idem-key-001";

    private static readonly CreatePaymentCommand ValidCommand = new(
        OrderId: OrderId,
        Provider: Provider,
        ProviderReference: null,
        IdempotencyKey: IdempotencyKey);

    private readonly CreatePaymentCommandHandler _handler;
    private readonly IPaymentRepository _paymentRepositoryMock;
    private readonly IOrderRepository _orderRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;

    public CreatePaymentTests()
    {
        _paymentRepositoryMock = Substitute.For<IPaymentRepository>();
        _orderRepositoryMock = Substitute.For<IOrderRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();

        _orderRepositoryMock
            .GetByIdAsync(OrderId, Arg.Any<CancellationToken>())
            .Returns(OrderData.CreateOrder());

        _handler = new CreatePaymentCommandHandler(
            _paymentRepositoryMock,
            _orderRepositoryMock,
            _unitOfWorkMock);
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
    public async Task Handle_Should_ReturnFailure_When_Provider_Is_Empty()
    {
        // Arrange
        var command = ValidCommand with { Provider = string.Empty };

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PaymentErrors.ProviderRequired);
        _paymentRepositoryMock.DidNotReceive().Add(Arg.Any<PaymentEntity>());
        await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_IdempotencyKey_Is_Empty()
    {
        // Arrange
        var command = ValidCommand with { IdempotencyKey = string.Empty };

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PaymentErrors.IdempotencyKeyRequired);
        _paymentRepositoryMock.DidNotReceive().Add(Arg.Any<PaymentEntity>());
        await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_And_PersistPayment_When_Command_Is_Valid()
    {
        // Arrange (defaults set in constructor)

        // Act
        var result = await _handler.Handle(ValidCommand, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        _paymentRepositoryMock.Received(1).Add(Arg.Any<PaymentEntity>());
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
