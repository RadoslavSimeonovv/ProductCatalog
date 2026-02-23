using ProductCatalog.Application.Messaging;
using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Order.Errors;
using ProductCatalog.Domain.Order.Repositories;
using ProductCatalog.Domain.Payment.Repositories;
using PaymentEntity = ProductCatalog.Domain.Payment.Entities.Payment;

namespace ProductCatalog.Application.Payment.CreatePayment;

internal sealed class CreatePaymentCommandHandler : ICommandHandler<CreatePaymentCommand, Guid>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePaymentCommandHandler(IPaymentRepository paymentRepository, IOrderRepository orderRepository, IUnitOfWork unitOfWork)
    {
        _paymentRepository = paymentRepository;
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);

        if (order == null)
            return Result.Failure<Guid>(OrderErrors.NotFound);

        var paymentResult = PaymentEntity.Create(
            request.OrderId,
            order.CustomerId,
            order.TotalAmount,
            request.Provider,
            request.IdempotencyKey
        );

        if (paymentResult.IsFailure)
            return Result.Failure<Guid>(paymentResult.Error);

        _paymentRepository.Add(paymentResult.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(paymentResult.Value.Id);
    }
}