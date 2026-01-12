using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Payment.Enums;
using ProductCatalog.Domain.Shared.ValueObjects;

namespace ProductCatalog.Domain.Payment.Entities;

public sealed class Payment : Entity
{
    private Payment(Guid id,
        Guid orderId,
        Money amount,
        string provider,
        string providerReference,
        string idemptencyKey) : base(id)
    {
        OrderId = orderId;
        Provider = provider;
        ProviderReference = providerReference;
        IdempotencyKey = idemptencyKey;
        Amount = amount;
        Status = PaymentStatus.Initiated;
    }

    public Guid OrderId { get; private set; }
    public Money Amount { get; private set; }
    public PaymentStatus Status { get; private set; }
    public string Provider { get; private set; }
    public string ProviderReference { get; private set; }
    public string IdempotencyKey { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public static Payment Create(Guid orderId,
        Money amount,
        string provider,
        string providerReference,
        string idemptencyKey)
    {
        return new Payment(Guid.NewGuid(),
            orderId,
            amount,
            provider,
            providerReference,
            idemptencyKey);
    }
}