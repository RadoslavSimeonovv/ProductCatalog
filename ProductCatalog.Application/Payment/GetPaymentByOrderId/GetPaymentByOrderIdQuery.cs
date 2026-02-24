using ProductCatalog.Application.Abstractions.Messaging;

namespace ProductCatalog.Application.Payment.GetPaymentByOrderId;

public sealed record GetPaymentByOrderIdQuery(Guid OrderId) 
    : IQuery<IReadOnlyList<PaymentResponse>>;