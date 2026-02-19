using ProductCatalog.Application.Messaging;
using ProductCatalog.Application.Order.Responses;
using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Order.Repositories;

namespace ProductCatalog.Application.Order.GetAllOrders;

internal sealed class GetAllOrdersQueryHandler : IQueryHandler<GetAllOrdersQuery, List<OrderResponse>>
{
    private readonly IOrderRepository _orderRepository;

    public GetAllOrdersQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }
    public async Task<Result<List<OrderResponse>>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetAllAsync(cancellationToken);

        if (orders.Count == 0)
            return Result.Success(new List<OrderResponse>());

        var orderResponses = orders.Select(order => new OrderResponse
        {
            Id = order.Id,
            CustomerEmail = order.CustomerEmail,
            Status = order.Status.ToString(),
            Items = order.Items.Select(item => new OrderItemResponse
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice.Amount,
                Currency = item.UnitPrice.Currency.Code
            }).ToList()
        }).ToList();

        return Result.Success(orderResponses);
    }
}
