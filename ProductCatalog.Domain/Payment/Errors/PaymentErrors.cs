using ProductCatalog.Domain.Abstractions;

namespace ProductCatalog.Domain.Payment.Errors;

public static class PaymentErrors
{
    // Generic
    public static readonly Error NotFound =
        new("Payment.NotFound", "Payment was not found.");

    public static readonly Error InvalidState =
        new("Payment.InvalidState", "Payment is in an invalid state for this operation.");

    // Creation
    public static readonly Error InvalidOrderId =
        new("Payment.InvalidOrderId", "OrderId cannot be empty.");

    public static readonly Error InvalidAmount =
        new("Payment.InvalidAmount", "Payment amount must be greater than zero.");

    public static readonly Error ProviderRequired =
        new("Payment.ProviderRequired", "Provider is required.");

    public static readonly Error IdempotencyKeyRequired =
        new("Payment.IdempotencyKeyRequired", "Idempotency key is required.");

    // Status transitions
    public static readonly Error AlreadySucceeded =
        new("Payment.AlreadySucceeded", "Payment is already marked as succeeded.");

    public static readonly Error AlreadyFailed =
        new("Payment.AlreadyFailed", "Payment is already marked as failed.");

    public static readonly Error CannotSucceedFailedPayment =
        new("Payment.CannotSucceedFailedPayment", "Cannot mark a failed payment as succeeded.");

    public static readonly Error CannotFailSucceededPayment =
        new("Payment.CannotFailSucceededPayment", "Cannot mark a succeeded payment as failed.");

    public static readonly Error ProviderReferenceRequired =
        new("Payment.ProviderReferenceRequired", "Provider reference is required.");
}
