namespace ProductCatalog.Domain.Payment.Repositories;

public interface IPaymentRepository
{
    Task<Entities.Payment?> GetByIdAsync(Guid paymentId, CancellationToken cancellationToken = default);
    void Add(Entities.Payment payment);
}