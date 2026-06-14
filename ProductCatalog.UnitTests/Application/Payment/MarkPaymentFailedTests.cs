using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using ProductCatalog.Application.Exceptions;
using ProductCatalog.Application.Payment.MarkPaymentFailed;
using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Payment.Errors;
using ProductCatalog.Domain.Payment.Repositories;
using ProductCatalog.UnitTests.Domain.Payment;
using PaymentEntity = ProductCatalog.Domain.Payment.Entities.Payment;

namespace ProductCatalog.UnitTests.Application.Payment;

public class MarkPaymentFailedTests
{
    private static readonly Guid PaymentId = PaymentData.CreatePayment().Id;
    private const string Reason = "Insufficient funds";

    private static readonly MarkPaymentFailedCommand ValidCommand = new(
        PaymentId: PaymentId,
        Reason: Reason);

    private readonly MarkPaymentFailedCommandHandler _handler;
    private readonly IPaymentRepository _paymentRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;

    public MarkPaymentFailedTests()
    {
        _paymentRepositoryMock = Substitute.For<IPaymentRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();

        _handler = new MarkPaymentFailedCommandHandler(_paymentRepositoryMock, _unitOfWorkMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_Payment_Not_Found()
    {
        // Arrange
        _paymentRepositoryMock
            .GetByIdAsync(PaymentId, Arg.Any<CancellationToken>())
            .Returns((PaymentEntity?)null);

        // Act
        var result = await _handler.Handle(ValidCommand, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PaymentErrors.NotFound);
        await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_Payment_Is_Already_Failed_Idempotent()
    {
        // Arrange
        // MarkAsFailed on an already-failed payment is idempotent — domain returns Success
        _paymentRepositoryMock
            .GetByIdAsync(PaymentId, Arg.Any<CancellationToken>())
            .Returns(PaymentData.CreateFailedPayment());

        // Act
        var result = await _handler.Handle(ValidCommand, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_Payment_Is_Already_Succeeded()
    {
        // Arrange
        _paymentRepositoryMock
            .GetByIdAsync(PaymentId, Arg.Any<CancellationToken>())
            .Returns(PaymentData.CreateSucceededPayment());

        // Act
        var result = await _handler.Handle(ValidCommand, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PaymentErrors.CannotFailSucceededPayment);
        await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ConcurrencyException_Is_Thrown()
    {
        // Arrange
        _paymentRepositoryMock
            .GetByIdAsync(PaymentId, Arg.Any<CancellationToken>())
            .Returns(PaymentData.CreatePayment());

        _unitOfWorkMock
            .SaveChangesAsync(Arg.Any<CancellationToken>())
            .ThrowsAsync(new ConcurrencyException("conflict", null!));

        // Act
        var result = await _handler.Handle(ValidCommand, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PaymentErrors.ConcurrencyConflict);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_Payment_Is_Initiated()
    {
        // Arrange
        _paymentRepositoryMock
            .GetByIdAsync(PaymentId, Arg.Any<CancellationToken>())
            .Returns(PaymentData.CreatePayment());

        // Act
        var result = await _handler.Handle(ValidCommand, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
