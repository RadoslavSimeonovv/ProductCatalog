using ProductCatalog.Application.Messaging;
using ProductCatalog.Application.Order.Responses;
using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Order.Errors;
using ProductCatalog.Domain.Order.Repositories;

namespace ProductCatalog.Application.Order.GetOrderById;

internal sealed class GetOrderByIdQueryHandler : IQueryHandler<GerOrderByIdQuery, OrderResponse>
{
    private readonly IOrderRepository _orderRepository;
    public GetOrderByIdQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }
    public async Task<Result<OrderResponse>> Handle(GerOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);

        if (order is null)
            return Result.Failure<OrderResponse>(OrderErrors.NotFound);

        var orderResponse = new OrderResponse()
        {
            CustomerEmail = order.CustomerEmail,
            Id = order.Id,
            Status = order.Status.ToString(),
            Items = order.Items.Select(i => new OrderItemResponse()
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice.Amount,
                Currency = i.UnitPrice.Currency.Code
            }).ToList()
        };

        return Result.Success(orderResponse);
    }
}