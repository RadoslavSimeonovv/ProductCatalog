using ProductCatalog.Application.Abstractions.Messaging;

namespace ProductCatalog.Application.Payment.GetPaymentById;

public sealed record GetPaymentByIdQuery(Guid PaymentId)
    : IQuery<PaymentResponse>;