using ProductCatalog.Application.Messaging;

namespace ProductCatalog.Application.Order.SubmitOrderForPayment;

public sealed record SubmitOrderForPaymentCommand(Guid OrderId) : ICommand;