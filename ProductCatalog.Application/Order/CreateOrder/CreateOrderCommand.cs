using ProductCatalog.Application.Abstractions.Messaging;

namespace ProductCatalog.Application.Order.CreateOrder;

public sealed record CreateOrderCommand(
    string CustomerEmail,
    List<CreateOrderItemDto> Items) : ICommand<Guid>;