using ProductCatalog.Application.Messaging;
using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Payment.Errors;
using ProductCatalog.Domain.Payment.Repositories;

namespace ProductCatalog.Application.Payment.MarkPaymentSucceeded;

internal sealed class MarkPaymentSucceededCommandHandler : ICommandHandler<MarkPaymentSucceededCommand>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MarkPaymentSucceededCommandHandler(IPaymentRepository paymentRepository, IUnitOfWork unitOfWork)
    {
        _paymentRepository = paymentRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<Result> Handle(MarkPaymentSucceededCommand request, CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken);

        if (payment is null)
            return Result.Failure(PaymentErrors.NotFound);

        var result = payment.MarkAsSucceeded(request.ProviderReference!);
        if (result.IsFailure)
            return result;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}