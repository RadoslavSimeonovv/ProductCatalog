namespace ProductCatalog.Domain.Payment.Repositories;

using ProductCatalog.Domain.Catalog.Entities;
using PaymentEntity = Entities.Payment;

public interface IPaymentRepository
{
    Task<PaymentEntity?> GetByIdAsync(Guid paymentId, CancellationToken cancellationToken = default);

    IQueryable<PaymentEntity> GetPaymentsByOrderId(Guid orderId);

    void Add(PaymentEntity payment);
}