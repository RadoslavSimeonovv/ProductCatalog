using PaymentEntity = ProductCatalog.Domain.Payment.Entities.Payment;
using ProductCatalog.Domain.Shared.ValueObjects;

namespace ProductCatalog.UnitTests.Domain.Payment;

internal static class PaymentData
{
    public static readonly Guid OrderId = new Guid("d1d2d3d4-0000-0000-0000-000000000001");
    public static readonly CustomerId CustomerId = CustomerId.Create("user-123").Value;
    public static readonly Money Amount = new Money(20.00m, new Currency("USD"));
    public static readonly string Provider = "Stripe";
    public static readonly string IdempotencyKey = "idem-key-001";
    public static readonly string ProviderReference = "pi_test_123";
    public static readonly string DifferentProviderReference = "pi_test_999";

    public static PaymentEntity CreatePayment() =>
        PaymentEntity.Create(OrderId, CustomerId, Amount, Provider, IdempotencyKey).Value;

    public static PaymentEntity CreateSucceededPayment()
    {
        var payment = CreatePayment();
        payment.MarkAsSucceeded(ProviderReference);
        return payment;
    }

    public static PaymentEntity CreateFailedPayment()
    {
        var payment = CreatePayment();
        payment.MarkAsFailed("Insufficient funds");
        return payment;
    }
}
