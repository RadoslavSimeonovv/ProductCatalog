namespace ProductCatalog.Domain.Order.Repositories;
using OrderEntity = Entities.Order;

public interface IOrderRepository
{
    Task<OrderEntity> GetByIdAsync(Guid orderId, CancellationToken cancellationToken = default);

    Task<List<OrderEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    void Add(OrderEntity order);
}