using ProductCatalog.Application.Abstractions.Authentication;
using ProductCatalog.Application.Abstractions.Messaging;
using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Order.Entities;
using ProductCatalog.Domain.Order.Errors;
using ProductCatalog.Domain.Order.Repositories;
using ProductCatalog.Domain.Shared.ValueObjects;
using OrderEntity = ProductCatalog.Domain.Order.Entities.Order;

namespace ProductCatalog.Application.Order.CreateOrder;

internal sealed class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, Guid>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public CreateOrderCommandHandler(
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }
    public async Task<Result<Guid>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated || string.IsNullOrWhiteSpace(_currentUser.UserId))
            return Result.Failure<Guid>(OrderErrors.Unauthorized);

        if (request.Items is null || request.Items.Count == 0)
            return Result.Failure<Guid>(OrderErrors.EmptyOrder);

        var customerIdResult = CustomerId.Create(_currentUser.UserId);
        if (customerIdResult.IsFailure)
            return Result.Failure<Guid>(customerIdResult.Error);

        var orderId = Guid.NewGuid();

        var items = new List<OrderItem>(request.Items.Count);

        foreach (var dto in request.Items)
        {
            var currency = Currency.FromCode(dto.Currency);
            var unitPrice = new Money(dto.UnitPriceAmount, currency);

            var item = new OrderItem(
                id: Guid.NewGuid(),
                orderId: orderId,
                productId: dto.ProductId,
                quantity: dto.Quantity,
                unitPrice: unitPrice);

            items.Add(item);
        }

        var createResult = OrderEntity.Create(orderId, customerIdResult.Value, request.CustomerEmail, items);
        if (createResult.IsFailure)
            return Result.Failure<Guid>(createResult.Error);

        _orderRepository.Add(createResult.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(orderId);
    }
}