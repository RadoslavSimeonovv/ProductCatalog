using ProductCatalog.Application.Messaging;

namespace ProductCatalog.Application.Payment.GetPaymentByOrderId;

public sealed record GetPaymentByOrderIdQuery(Guid OrderId) 
    : IQuery<IReadOnlyList<PaymentResponse>>;