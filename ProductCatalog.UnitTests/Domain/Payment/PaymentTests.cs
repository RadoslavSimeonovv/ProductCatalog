using PaymentEntity = ProductCatalog.Domain.Payment.Entities.Payment;
using FluentAssertions;
using ProductCatalog.Domain.Payment.Enums;
using ProductCatalog.Domain.Payment.Errors;
using ProductCatalog.Domain.Payment.Events;
using ProductCatalog.Domain.Shared.ValueObjects;

namespace ProductCatalog.UnitTests.Domain.Payment;

public class PaymentTests
{
    #region Create

    [Fact]
    public void Create_Should_ReturnFailure_WhenOrderIdIsEmpty()
    {
        // Act
        var result = PaymentEntity.Create(Guid.Empty, PaymentData.CustomerId, PaymentData.Amount, PaymentData.Provider, PaymentData.IdempotencyKey);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PaymentErrors.InvalidOrderId);
    }

    [Fact]
    public void Create_Should_ReturnFailure_WhenAmountIsNull()
    {
        // Act
        var result = PaymentEntity.Create(PaymentData.OrderId, PaymentData.CustomerId, null!, PaymentData.Provider, PaymentData.IdempotencyKey);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PaymentErrors.InvalidAmount);
    }

    [Fact]
    public void Create_Should_ReturnFailure_WhenAmountIsZero()
    {
        // Arrange
        var zeroAmount = new Money(0m, new Currency("USD"));

        // Act
        var result = PaymentEntity.Create(PaymentData.OrderId, PaymentData.CustomerId, zeroAmount, PaymentData.Provider, PaymentData.IdempotencyKey);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PaymentErrors.InvalidAmount);
    }

    [Fact]
    public void Create_Should_ReturnFailure_WhenProviderIsEmpty()
    {
        // Act
        var result = PaymentEntity.Create(PaymentData.OrderId, PaymentData.CustomerId, PaymentData.Amount, "", PaymentData.IdempotencyKey);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PaymentErrors.ProviderRequired);
    }

    [Fact]
    public void Create_Should_ReturnFailure_WhenIdempotencyKeyIsEmpty()
    {
        // Act
        var result = PaymentEntity.Create(PaymentData.OrderId, PaymentData.CustomerId, PaymentData.Amount, PaymentData.Provider, "");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PaymentErrors.IdempotencyKeyRequired);
    }

    [Fact]
    public void Create_Should_ReturnSuccess_WhenAllParametersAreValid()
    {
        // Act
        var result = PaymentEntity.Create(PaymentData.OrderId, PaymentData.CustomerId, PaymentData.Amount, PaymentData.Provider, PaymentData.IdempotencyKey);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.OrderId.Should().Be(PaymentData.OrderId);
        result.Value.CustomerId.Should().Be(PaymentData.CustomerId);
        result.Value.Amount.Should().Be(PaymentData.Amount);
        result.Value.Provider.Should().Be(PaymentData.Provider);
        result.Value.IdempotencyKey.Should().Be(PaymentData.IdempotencyKey);
        result.Value.Status.Should().Be(PaymentStatus.Initiated);
    }

    [Fact]
    public void Create_Should_RaiseDomainEvent_WhenPaymentIsCreated()
    {
        // Act
        var result = PaymentEntity.Create(PaymentData.OrderId, PaymentData.CustomerId, PaymentData.Amount, PaymentData.Provider, PaymentData.IdempotencyKey);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var evt = result.Value.GetDomainEvents().OfType<PaymentInitiatedDomainEvent>().SingleOrDefault();
        evt.Should().NotBeNull();
        evt!.PaymentId.Should().Be(result.Value.Id);
        evt.OrderId.Should().Be(PaymentData.OrderId);
        evt.Amount.Should().Be(PaymentData.Amount);
        evt.Provider.Should().Be(PaymentData.Provider);
        evt.IdempotencyKey.Should().Be(PaymentData.IdempotencyKey);
    }

    #endregion

    #region MarkAsSucceeded

    [Fact]
    public void MarkAsSucceeded_Should_ReturnSuccess_WhenPaymentIsInitiated()
    {
        // Arrange
        var payment = PaymentData.CreatePayment();

        // Act
        var result = payment.MarkAsSucceeded(PaymentData.ProviderReference);

        // Assert
        result.IsSuccess.Should().BeTrue();
        payment.Status.Should().Be(PaymentStatus.Succeeded);
        payment.ProviderReference.Should().Be(PaymentData.ProviderReference);
    }

    [Fact]
    public void MarkAsSucceeded_Should_ReturnFailure_WhenProviderReferenceIsEmpty()
    {
        // Arrange
        var payment = PaymentData.CreatePayment();

        // Act
        var result = payment.MarkAsSucceeded("");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PaymentErrors.ProviderReferenceRequired);
    }

    [Fact]
    public void MarkAsSucceeded_Should_ReturnFailure_WhenAlreadySucceededWithDifferentReference()
    {
        // Arrange
        var payment = PaymentData.CreateSucceededPayment();

        // Act
        var result = payment.MarkAsSucceeded(PaymentData.DifferentProviderReference);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PaymentErrors.AlreadySucceeded);
    }

    [Fact]
    public void MarkAsSucceeded_Should_ReturnSuccess_WhenAlreadySucceededWithSameReference()
    {
        // Arrange — idempotent retry with same reference
        var payment = PaymentData.CreateSucceededPayment();

        // Act
        var result = payment.MarkAsSucceeded(PaymentData.ProviderReference);

        // Assert
        result.IsSuccess.Should().BeTrue();
        payment.Status.Should().Be(PaymentStatus.Succeeded);
    }

    [Fact]
    public void MarkAsSucceeded_Should_ReturnFailure_WhenPaymentIsFailed()
    {
        // Arrange
        var payment = PaymentData.CreateFailedPayment();

        // Act
        var result = payment.MarkAsSucceeded(PaymentData.ProviderReference);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PaymentErrors.CannotSucceedFailedPayment);
    }

    [Fact]
    public void MarkAsSucceeded_Should_RaiseDomainEvent_WhenPaymentSucceeds()
    {
        // Arrange
        var payment = PaymentData.CreatePayment();

        // Act
        payment.MarkAsSucceeded(PaymentData.ProviderReference);

        // Assert
        var evt = payment.GetDomainEvents().OfType<PaymentSucceededDomainEvent>().SingleOrDefault();
        evt.Should().NotBeNull();
        evt!.PaymentId.Should().Be(payment.Id);
        evt.OrderId.Should().Be(PaymentData.OrderId);
        evt.Amount.Should().Be(PaymentData.Amount);
        evt.ProviderReference.Should().Be(PaymentData.ProviderReference);
    }

    #endregion

    #region MarkAsFailed

    [Fact]
    public void MarkAsFailed_Should_ReturnSuccess_WhenPaymentIsInitiated()
    {
        // Arrange
        var payment = PaymentData.CreatePayment();

        // Act
        var result = payment.MarkAsFailed("Insufficient funds");

        // Assert
        result.IsSuccess.Should().BeTrue();
        payment.Status.Should().Be(PaymentStatus.Failed);
    }

    [Fact]
    public void MarkAsFailed_Should_ReturnFailure_WhenPaymentIsAlreadySucceeded()
    {
        // Arrange
        var payment = PaymentData.CreateSucceededPayment();

        // Act
        var result = payment.MarkAsFailed("Something went wrong");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PaymentErrors.CannotFailSucceededPayment);
    }

    [Fact]
    public void MarkAsFailed_Should_ReturnSuccess_WhenPaymentIsAlreadyFailed()
    {
        // Arrange — idempotent
        var payment = PaymentData.CreateFailedPayment();

        // Act
        var result = payment.MarkAsFailed("Another reason");

        // Assert
        result.IsSuccess.Should().BeTrue();
        payment.Status.Should().Be(PaymentStatus.Failed);
    }

    [Fact]
    public void MarkAsFailed_Should_RaiseDomainEvent_WhenPaymentFails()
    {
        // Arrange
        var payment = PaymentData.CreatePayment();
        var reason = "Card declined";

        // Act
        payment.MarkAsFailed(reason);

        // Assert
        var evt = payment.GetDomainEvents().OfType<PaymentFailedDomainEvent>().SingleOrDefault();
        evt.Should().NotBeNull();
        evt!.PaymentId.Should().Be(payment.Id);
        evt.OrderId.Should().Be(PaymentData.OrderId);
        evt.Amount.Should().Be(PaymentData.Amount);
        evt.FailureReason.Should().Be(reason);
    }

    [Fact]
    public void MarkAsFailed_Should_SetReasonToNull_WhenReasonIsWhitespace()
    {
        // Arrange
        var payment = PaymentData.CreatePayment();

        // Act
        payment.MarkAsFailed("   ");

        // Assert
        var evt = payment.GetDomainEvents().OfType<PaymentFailedDomainEvent>().SingleOrDefault();
        evt.Should().NotBeNull();
        evt!.FailureReason.Should().BeNull();
    }

    #endregion
}
