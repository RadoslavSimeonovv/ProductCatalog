using ProductCatalog.Application.Abstractions.Messaging;

namespace ProductCatalog.Application.Payment.GetPaymentsByOrderId;

public sealed record GetPaymentsByOrderIdQuery(Guid OrderId) 
    : IQuery<IReadOnlyList<PaymentResponse>>;