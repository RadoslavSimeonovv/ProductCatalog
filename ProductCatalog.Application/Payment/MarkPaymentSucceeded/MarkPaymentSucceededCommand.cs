using ProductCatalog.Application.Messaging;

namespace ProductCatalog.Application.Payment.MarkPaymentSucceeded;

public sealed record MarkPaymentSucceededCommand(
    Guid PaymentId,
    string? ProviderReference) : ICommand;
