using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Payment.Enums;
using ProductCatalog.Domain.Shared.ValueObjects;

namespace ProductCatalog.Domain.Payment.Entities;

public class Payment : Entity
{
    public Payment(Guid id) : base(id)
    {
    }

    public Guid OrderId { get; private set; }
    public Money Amount { get; private set; }
    public PaymentStatus Status { get; private set; }
    public string Provider { get; private set; }
    public string ProviderReference { get; private set; }
    public string IdempotencyKey { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

}