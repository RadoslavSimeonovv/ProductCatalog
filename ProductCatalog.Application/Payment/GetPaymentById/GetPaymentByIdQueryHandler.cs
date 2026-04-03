using ProductCatalog.Application.Abstractions.Authentication;
using ProductCatalog.Application.Abstractions.Messaging;
using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Payment.Errors;
using ProductCatalog.Domain.Payment.Repositories;

namespace ProductCatalog.Application.Payment.GetPaymentById;

internal sealed class GetPaymentByIdQueryHandler : IQueryHandler<GetPaymentByIdQuery, PaymentResponse?>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly ICurrentUser _currentUser;

    public GetPaymentByIdQueryHandler(IPaymentRepository paymentRepository, ICurrentUser currentUser)
    {
        _paymentRepository = paymentRepository;
        _currentUser = currentUser;
    }
    public async Task<Result<PaymentResponse?>> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken);

        if (payment is null)
            return Result.Failure<PaymentResponse?>(PaymentErrors.NotFound);

        if (!_currentUser.IsInRole(Roles.Admin) && payment.CustomerId.Value != _currentUser.UserId)
            return Result.Failure<PaymentResponse?>(PaymentErrors.Unauthorized);

        var response = new PaymentResponse()
        {
            PaymentId = payment.Id,
            OrderId = payment.OrderId,
            Status = payment.Status,
            CustomerId = payment.CustomerId.Value,
            Amount = payment.Amount.Amount,
            Currency = payment.Amount.Currency.Code,
            Provider = payment.Provider,
            ProviderReference = payment.ProviderReference
        };

        return Result.Success(response)!;
    }
}