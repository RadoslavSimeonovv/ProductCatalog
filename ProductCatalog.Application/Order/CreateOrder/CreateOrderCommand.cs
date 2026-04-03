using ProductCatalog.Application.Abstractions.Messaging;

namespace ProductCatalog.Application.Order.CreateOrder;

public sealed record CreateOrderCommand(
    List<CreateOrderItemDto> Items) : ICommand<Guid>;