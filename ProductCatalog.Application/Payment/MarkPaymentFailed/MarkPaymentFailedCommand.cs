using ProductCatalog.Application.Abstractions.Messaging;

namespace ProductCatalog.Application.Payment.MarkPaymentFailed;

public sealed record MarkPaymentFailedCommand(
    Guid PaymentId,
    string? Reason) : ICommand;