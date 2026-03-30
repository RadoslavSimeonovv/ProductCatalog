using ProductCatalog.Application.Abstractions.Authentication;
using ProductCatalog.Application.Abstractions.Messaging;
using ProductCatalog.Application.Order.Responses;
using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Order.Errors;
using ProductCatalog.Domain.Order.Repositories;

namespace ProductCatalog.Application.Order.GetOrderById;

internal sealed class GetOrderByIdQueryHandler : IQueryHandler<GetOrderByIdQuery, OrderResponse>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICurrentUser _currentUser;

    public GetOrderByIdQueryHandler(IOrderRepository orderRepository, ICurrentUser currentUser)
    {
        _orderRepository = orderRepository;
        _currentUser = currentUser;
    }
    public async Task<Result<OrderResponse>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.Id, cancellationToken);

        if (order is null)
            return Result.Failure<OrderResponse>(OrderErrors.NotFound);

        if (!_currentUser.IsInRole(Roles.Admin) && order.CustomerId.Value != _currentUser.UserId)
            return Result.Failure<OrderResponse>(OrderErrors.Unauthorized);

        var orderResponse = new OrderResponse()
        {
            Id = order.Id,
            CustomerId = order.CustomerId.Value,
            CustomerEmail = order.CustomerEmail,
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