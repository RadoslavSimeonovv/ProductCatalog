using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Payment.Enums;
using ProductCatalog.Domain.Payment.Events;
using ProductCatalog.Domain.Shared.ValueObjects;

namespace ProductCatalog.Domain.Payment.Entities;

public sealed class Payment : Entity
{
    private Payment(Guid id,
        Guid orderId,
        Money amount,
        string provider,
        string providerReference,
        string idemptencyKey,
        DateTime createdAt) : base(id)
    {
        OrderId = orderId;
        Provider = provider;
        ProviderReference = providerReference;
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
    /// Creates a new payment instance for the specified order with the given details.
    /// </summary>
    /// <remarks>This method raises a domain event to signal that a payment has been initiated. Use the
    /// idempotency key to prevent duplicate payments for the same order in case of repeated requests.</remarks>
    /// <param name="orderId">The unique identifier of the order for which the payment is being created. Must not be empty.</param>
    /// <param name="amount">The amount to be paid. Must be a non-null value greater than zero.</param>
    /// <param name="provider">The name of the payment provider processing the payment. Cannot be null or empty.</param>
    /// <param name="providerReference">The reference identifier provided by the payment provider. Cannot be null or empty.</param>
    /// <param name="idemptencyKey">A unique key used to ensure idempotency of the payment creation request. Cannot be null or empty.</param>
    /// <returns>A new instance of the Payment class representing the initiated payment.</returns>
    /// <exception cref="ArgumentException">Thrown if any argument is invalid: <paramref name="orderId"/> is empty; <paramref name="amount"/> is null or not
    /// greater than zero; <paramref name="provider"/>, <paramref name="providerReference"/>, or <paramref
    /// name="idemptencyKey"/> is null or empty.</exception>
    public static Payment Create(Guid orderId,
        Money amount,
        string provider,
        string? providerReference,
        string idempotencyKey)
    {
        if (orderId == Guid.Empty)
        {
            throw new ArgumentException("Order ID cannot be empty.", nameof(orderId));
        }

        if (amount == null || amount.Amount <= 0)
        {
            throw new ArgumentNullException("Amount must be greater than zero and not null.", nameof(amount));
        }

        if (string.IsNullOrWhiteSpace(provider))
        {
            throw new ArgumentException("Provider cannot be null or empty.", nameof(provider));
        }

        if (string.IsNullOrWhiteSpace(idempotencyKey))
        {
            throw new ArgumentException("Idempotency key cannot be null or empty.", nameof(idempotencyKey));
        }

        var dateTimeNow = DateTime.UtcNow;

        var payment = new Payment(Guid.NewGuid(),
            orderId,
            amount,
            provider.Trim(),
            providerReference: null!,
            idempotencyKey.Trim(),
            dateTimeNow);

        payment.RaiseDomainEvent(new PaymentInitiatedDomainEvent(
            payment.Id,
            payment.OrderId,
            payment.Amount,
            payment.Provider,
            payment.IdempotencyKey,
            dateTimeNow));

        return payment;
    }

    /// <summary>
    /// Marks the payment as succeeded using the specified provider reference.
    /// </summary>
    /// <param name="providerReference">The unique reference provided by the payment provider to identify the successful transaction. Cannot be null,
    /// empty, or consist only of white-space characters.</param>
    /// <exception cref="ArgumentException">Thrown if providerReference is null, empty, or consists only of white-space characters.</exception>
    public void MarkAsSucceeded(string providerReference)
    {
        if (string.IsNullOrWhiteSpace(providerReference))
        {
            throw new ArgumentException("Provider reference cannot be null or empty.", nameof(providerReference));
        }

        if (Status == PaymentStatus.Succeeded)
        {
            if (ProviderReference == providerReference)
                return;

            throw new InvalidOperationException("Payment already succeeded with a different provider reference.");
        }

        if (Status == PaymentStatus.Failed)
        {
            throw new InvalidOperationException("Cannot mark a failed payment as succeeded.");
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
    }

    /// <summary>
    /// Marks the payment as failed and records the specified reason.
    /// </summary>
    /// <remarks>This method updates the payment status to failed and triggers a domain event to notify
    /// subscribers of the failure. The failure reason is included in the event if provided.</remarks>
    /// <param name="reason">The reason for the payment failure, or null if no specific reason is provided.</param>
    public void MarkAsFailed(string? reason)
    {
        if (Status == PaymentStatus.Succeeded)
        {
            throw new InvalidOperationException("Cannot mark a succeeded payment as failed.");
        }

        if (Status == PaymentStatus.Failed)
        {
            return;
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
    }
}