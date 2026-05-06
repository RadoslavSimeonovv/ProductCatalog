using Dapper;
using ProductCatalog.Application.Abstractions.Authentication;
using ProductCatalog.Application.Abstractions.Messaging;
using ProductCatalog.Application.Data;
using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Payment.Errors;

namespace ProductCatalog.Application.Payment.GetPaymentById;

internal sealed class GetPaymentByIdQueryHandler(ISqlConnectionFactory sqlConnectionFactory, ICurrentUser currentUser)
    : IQueryHandler<GetPaymentByIdQuery, PaymentResponse>
{
    public async Task<Result<PaymentResponse>> Handle(
        GetPaymentByIdQuery request,
        CancellationToken cancellationToken)
    {
        using var connection = sqlConnectionFactory.CreateConnection();

        const string sql = """
            SELECT
                id                 AS PaymentId,
                order_id           AS OrderId,
                customer_id        AS CustomerId,
                amount             AS Amount,
                currency           AS Currency,
                provider           AS Provider,
                provider_reference AS ProviderReference,
                status             AS Status
            FROM payments
            WHERE id = @PaymentId
            """;

        var payment = await connection.QueryFirstOrDefaultAsync<PaymentResponse>(
            new CommandDefinition(sql, new { request.PaymentId }, cancellationToken: cancellationToken));

        if (payment is null)
            return Result.Failure<PaymentResponse>(PaymentErrors.NotFound);

        if (!currentUser.IsInRole(Roles.Admin) && payment.CustomerId != currentUser.UserId)
            return Result.Failure<PaymentResponse>(PaymentErrors.Unauthorized);

        return Result.Success(payment);
    }
}
