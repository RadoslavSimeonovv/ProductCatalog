using ProductCatalog.Application.Abstractions.Authentication;
using ProductCatalog.Application.Abstractions.Messaging;
using ProductCatalog.Application.Exceptions;
using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Order.Errors;
using ProductCatalog.Domain.Order.Repositories;

namespace ProductCatalog.Application.Order.CancelOrder;

internal sealed class CancelOrderCommandHandler : ICommandHandler<CancelOrderCommand>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public CancelOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork, ICurrentUser currentUser)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }
    public async Task<Result> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);

        if (order == null)
            return Result.Failure(OrderErrors.NotFound);

        if (!_currentUser.IsInRole(Roles.Admin) && order.CustomerId.Value != _currentUser.UserId)
            return Result.Failure(OrderErrors.Unauthorized);

        var result = order.Cancel(request.Reason);

        if (result.IsFailure)
            return Result.Failure(result.Error);

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (ConcurrencyException)
        {
            return Result.Failure(OrderErrors.ConcurrencyConflict);
        }
    }
}