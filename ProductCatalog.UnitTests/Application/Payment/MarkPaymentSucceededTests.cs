using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using ProductCatalog.Application.Exceptions;
using ProductCatalog.Application.Payment.MarkPaymentSucceeded;
using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Payment.Errors;
using ProductCatalog.Domain.Payment.Repositories;
using ProductCatalog.UnitTests.Domain.Payment;
using PaymentEntity = ProductCatalog.Domain.Payment.Entities.Payment;

namespace ProductCatalog.UnitTests.Application.Payment;

public class MarkPaymentSucceededTests
{
    private static readonly Guid PaymentId = PaymentData.CreatePayment().Id;
    private const string ProviderReference = "pi_test_123";

    private static readonly MarkPaymentSucceededCommand ValidCommand = new(
        PaymentId: PaymentId,
        ProviderReference: ProviderReference);

    private readonly MarkPaymentSucceededCommandHandler _handler;
    private readonly IPaymentRepository _paymentRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;

    public MarkPaymentSucceededTests()
    {
        _paymentRepositoryMock = Substitute.For<IPaymentRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();

        _handler = new MarkPaymentSucceededCommandHandler(_paymentRepositoryMock, _unitOfWorkMock);
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
    public async Task Handle_Should_ReturnFailure_When_Payment_Is_Already_Succeeded_With_Different_Reference()
    {
        // Arrange
        // CreateSucceededPayment uses PaymentData.ProviderReference ("pi_test_123")
        // Command uses a different reference to trigger the conflict
        _paymentRepositoryMock
            .GetByIdAsync(PaymentId, Arg.Any<CancellationToken>())
            .Returns(PaymentData.CreateSucceededPayment());

        var command = new MarkPaymentSucceededCommand(PaymentId, PaymentData.DifferentProviderReference);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PaymentErrors.AlreadySucceeded);
        await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_Payment_Is_Already_Failed()
    {
        // Arrange
        _paymentRepositoryMock
            .GetByIdAsync(PaymentId, Arg.Any<CancellationToken>())
            .Returns(PaymentData.CreateFailedPayment());

        // Act
        var result = await _handler.Handle(ValidCommand, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PaymentErrors.CannotSucceedFailedPayment);
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
