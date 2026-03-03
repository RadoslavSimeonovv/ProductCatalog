using Microsoft.EntityFrameworkCore;
using ProductCatalog.Application.Abstractions.Messaging;
using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Payment.Repositories;

namespace ProductCatalog.Application.Payment.GetPaymentsByOrderId;

internal sealed class GetPaymentsByOrderIdQueryHandler : IQueryHandler<GetPaymentsByOrderIdQuery, IReadOnlyList<PaymentResponse>>
{
    private readonly IPaymentRepository _paymentRepository;

    public GetPaymentsByOrderIdQueryHandler(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }
    public async Task<Result<IReadOnlyList<PaymentResponse>>> Handle(GetPaymentsByOrderIdQuery request, CancellationToken cancellationToken)
    {
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