using Microsoft.EntityFrameworkCore;
using ProductCatalog.Application.Abstractions.Authentication;
using ProductCatalog.Application.Abstractions.Messaging;
using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Order.Errors;
using ProductCatalog.Domain.Order.Repositories;
using ProductCatalog.Domain.Payment.Repositories;

namespace ProductCatalog.Application.Payment.GetPaymentsByOrderId;

internal sealed class GetPaymentsByOrderIdQueryHandler : IQueryHandler<GetPaymentsByOrderIdQuery, IReadOnlyList<PaymentResponse>>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly ICurrentUser _currentUser;

    public GetPaymentsByOrderIdQueryHandler(
        IPaymentRepository paymentRepository,
        IOrderRepository orderRepository,
        ICurrentUser currentUser)
    {
        _paymentRepository = paymentRepository;
        _orderRepository = orderRepository;
        _currentUser = currentUser;
    }
    public async Task<Result<IReadOnlyList<PaymentResponse>>> Handle(GetPaymentsByOrderIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);

        if (order is null)
            return Result.Failure<IReadOnlyList<PaymentResponse>>(OrderErrors.NotFound);

        if (!_currentUser.IsInRole(Roles.Admin) && order.CustomerId.Value != _currentUser.UserId)
            return Result.Failure<IReadOnlyList<PaymentResponse>>(OrderErrors.Unauthorized);

        var paymentsResponse = await _paymentRepository.GetPaymentsByOrderId(request.OrderId)
            .AsNoTracking()
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new PaymentResponse
            {
                PaymentId = p.Id,
                OrderId = p.OrderId,
                CustomerId = p.CustomerId.Value,
                Amount = p.Amount.Amount,
                Currency = p.Amount.Currency.Code,
                Provider = p.Provider,
                ProviderReference = p.ProviderReference,
                Status = p.Status,
            })
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyList<PaymentResponse>>(paymentsResponse);
    }
}