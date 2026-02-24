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

    public CreateOrderCommandHandler(
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<Result<Guid>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        if (request.Items is null || request.Items.Count == 0)
            return Result.Failure<Guid>(OrderErrors.EmptyOrder);

        var customerIdResult = CustomerId.Create(request.CustomerId);
        if (customerIdResult.IsFailure)
            return Result.Failure<Guid>(customerIdResult.Error);

        var orderId = Guid.NewGuid();

        var items = new List<OrderItem>(request.Items.Count);

        foreach (var dto in request.Items)
        {
            var currency = new Currency(dto.Currency);
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