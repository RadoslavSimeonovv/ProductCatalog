using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Payment.Enums;
using ProductCatalog.Domain.Payment.Errors;
using ProductCatalog.Domain.Payment.Events;
using ProductCatalog.Domain.Shared.ValueObjects;

namespace ProductCatalog.Domain.Payment.Entities;

public sealed class Payment : Entity
{
    private Payment(Guid id,
        Guid orderId,
        Money amount,
        string provider,
        string? providerReference,
        string idemptencyKey,
        DateTime createdAt) : base(id)
    {
        OrderId = orderId;
        Provider = provider;
        ProviderReference = providerReference!;
        IdempotencyKey = idemptencyKey;
        Amount = amount;
        Status = PaymentStatus.Initiated;
        CreatedAt = createdAt;
    }

    public Guid OrderId { get; private set; }
    public Money Amount { get; private set; }
    public PaymentStatus Status { get; private set; }
    public string Provider { get; private set; }
    public string ProviderReference { get; private set; }
    public string IdempotencyKey { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    /// <summary>
    /// Static factory method to create a new Payment instance.
    /// </summary>
    /// <param name="orderId"></param>
    /// <param name="amount"></param>
    /// <param name="provider"></param>
    /// <param name="providerReference"></param>
    /// <param name="idempotencyKey"></param>
    /// <returns></returns>
    public static Result<Payment> Create(Guid orderId,
        Money amount,
        string provider,
        string idempotencyKey)
    {
        if (orderId == Guid.Empty)
        {
            return Result.Failure<Payment>(PaymentErrors.InvalidOrderId);
        }

        if (amount == null || amount.Amount <= 0)
        {
            return Result.Failure<Payment>(PaymentErrors.InvalidAmount);
        }

        if (string.IsNullOrWhiteSpace(provider))
        {
            return Result.Failure<Payment>(PaymentErrors.ProviderRequired);
        }

        if (string.IsNullOrWhiteSpace(idempotencyKey))
        {
            return Result.Failure<Payment>(PaymentErrors.IdempotencyKeyRequired);
        }

        var dateTimeNow = DateTime.UtcNow;

        var payment = new Payment(Guid.NewGuid(),
            orderId,
            amount,
            provider.Trim(),
            providerReference: null,
            idempotencyKey.Trim(),
            dateTimeNow);

        payment.RaiseDomainEvent(new PaymentInitiatedDomainEvent(
            payment.Id,
            payment.OrderId,
            payment.Amount,
            payment.Provider,
            payment.IdempotencyKey,
            dateTimeNow));

        return Result.Success<Payment>(payment);
    }

    /// <summary>
    /// Marks the payment as succeeded with the specified provider reference.
    /// </summary>
    /// <param name="providerReference"></param>
    /// <returns></returns>
    public Result MarkAsSucceeded(string providerReference)
    {
        if (string.IsNullOrWhiteSpace(providerReference))
        {
            return Result.Failure(PaymentErrors.ProviderReferenceRequired);
        }

        if (Status == PaymentStatus.Succeeded)
        {
            providerReference = providerReference.Trim();
            if (ProviderReference == providerReference)
                return Result.Success();

            return Result.Failure(PaymentErrors.AlreadySucceeded);
        }

        if (Status == PaymentStatus.Failed)
        {
            return Result.Failure(PaymentErrors.CannotSucceedFailedPayment);
        }

        var dateTimeNow = DateTime.UtcNow;

        ProviderReference = providerReference.Trim();
        Status = PaymentStatus.Succeeded;

        UpdatedAt = dateTimeNow;

        RaiseDomainEvent(new PaymentSucceededDomainEvent(
            Id,
            OrderId,
            Amount,
            ProviderReference,
            dateTimeNow));

        return Result.Success();
    }

    /// <summary>
    /// Marks the payment as failed with an optional reason.
    /// </summary>
    /// <param name="reason"></param>
    /// <returns></returns>
    public Result MarkAsFailed(string? reason)
    {
        if (Status == PaymentStatus.Succeeded)
        {
            return Result.Failure(PaymentErrors.CannotFailSucceededPayment);
        }

        if (Status == PaymentStatus.Failed)
        {
            return Result.Success();
        }

        var dateTimeNow = DateTime.UtcNow;
        reason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();

        Status = PaymentStatus.Failed;
        UpdatedAt = dateTimeNow;

        RaiseDomainEvent(new PaymentFailedDomainEvent(
            Id,
            OrderId,
            Amount,
            reason,
            dateTimeNow));
        
        return Result.Success();
    }
}