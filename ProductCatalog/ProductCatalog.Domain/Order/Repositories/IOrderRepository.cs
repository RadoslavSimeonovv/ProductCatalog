namespace ProductCatalog.Domain.Order.Repositories;

public interface IOrderRepository
{
    Task<Entities.Order> GetByIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    void Add(Entities.Order order);
}