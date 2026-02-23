using ProductCatalog.Application.Messaging;

namespace ProductCatalog.Application.Payment.MarkPaymentFailed;

public sealed record MarkPaymentFailedCommand(
    Guid PaymentId,
    string? Reason) : ICommand;