namespace ProductCatalog.Domain.Order.Repositories;
using OrderEntity = Entities.Order;

public interface IOrderRepository
{
    Task<OrderEntity?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    void Add(OrderEntity order);
}