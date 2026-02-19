using ProductCatalog.Application.Messaging;

namespace ProductCatalog.Application.Order.CreateOrder;

public sealed record CreateOrderCommand(
    string CustomerEmail,
    string CustomerId,
    List<CreateOrderItemDto> Items) : ICommand<Guid>;