using ProductCatalog.Application.Messaging;

namespace ProductCatalog.Application.Payment.CreatePayment;

public sealed record CreatePaymentCommand(
    Guid OrderId,
    string Provider,
    string? ProviderReference,
    string IdempotencyKey) : ICommand<Guid>;