using ProductCatalog.Application.Abstractions.Messaging;
using ProductCatalog.Application.Exceptions;
using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Order.Errors;
using ProductCatalog.Domain.Order.Repositories;
using ProductCatalog.Domain.Payment.Repositories;
using PaymentEntity = ProductCatalog.Domain.Payment.Entities.Payment;

namespace ProductCatalog.Application.Order.SubmitOrderForPayment;

internal sealed class SubmitOrderForPaymentCommandHandler : ICommandHandler<SubmitOrderForPaymentCommand, Guid>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SubmitOrderForPaymentCommandHandler(
        IOrderRepository orderRepository,
        IPaymentRepository paymentRepository,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _paymentRepository = paymentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(SubmitOrderForPaymentCommand request, CancellationToken cancellationToken)
    {
        var existingPayment = await _paymentRepository
            .GetByIdempotencyKeyAsync(request.IdempotencyKey, request.Provider, cancellationToken);

        if (existingPayment is not null)
            return Result.Success(existingPayment.Id);

        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);

        if (order == null)
            return Result.Failure<Guid>(OrderErrors.NotFound);

        var result = order.SubmitForPayment();

        if (result.IsFailure)
            return Result.Failure<Guid>(result.Error);

        var paymentResult = PaymentEntity.Create(
            orderId: order.Id,
            customerId: order.CustomerId,
            amount: order.TotalAmount,
            provider: request.Provider,
            idempotencyKey: request.IdempotencyKey);

        if (paymentResult.IsFailure) return Result.Failure<Guid>(paymentResult.Error);

        _paymentRepository.Add(paymentResult.Value);

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(paymentResult.Value.Id);
        }
        catch (ConcurrencyException)
        {
            return Result.Failure<Guid>(OrderErrors.ConcurrencyConflict);
        }
    }
}