using ProductCatalog.Application.Abstractions.Messaging;
using ProductCatalog.Application.Order.Responses;

namespace ProductCatalog.Application.Order.GetOrderById;

public sealed record GetOrderByIdQuery(Guid Id) : IQuery<OrderResponse>;
