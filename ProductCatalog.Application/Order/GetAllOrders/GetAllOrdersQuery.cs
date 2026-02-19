using ProductCatalog.Application.Messaging;
using ProductCatalog.Application.Order.Responses;

namespace ProductCatalog.Application.Order.GetAllOrders;

public sealed record GetAllOrdersQuery() : IQuery<List<OrderResponse>>;