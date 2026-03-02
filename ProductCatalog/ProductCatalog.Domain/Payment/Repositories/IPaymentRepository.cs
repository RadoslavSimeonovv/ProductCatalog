using PaymentEntity = ProductCatalog.Domain.Payment.Entities.Payment;

namespace ProductCatalog.Domain.Payment.Repositories;

public interface IPaymentRepository
{
    Task<PaymentEntity?> GetByIdAsync(Guid paymentId, CancellationToken cancellationToken = default);

    IQueryable<PaymentEntity> GetPaymentsByOrderId(Guid orderId);

    void Add(PaymentEntity payment);

    Task<PaymentEntity?> GetByIdempotencyKeyAsync(string key, string provider, CancellationToken cancellationToken = default);
}