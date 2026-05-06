using Dapper;
using ProductCatalog.Application.Abstractions.Authentication;
using ProductCatalog.Application.Abstractions.Messaging;
using ProductCatalog.Application.Data;
using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Order.Errors;

namespace ProductCatalog.Application.Payment.GetPaymentsByOrderId;

internal sealed class GetPaymentsByOrderIdQueryHandler(ISqlConnectionFactory sqlConnectionFactory, ICurrentUser currentUser)
    : IQueryHandler<GetPaymentsByOrderIdQuery, IReadOnlyList<PaymentResponse>>
{
    public async Task<Result<IReadOnlyList<PaymentResponse>>> Handle(
        GetPaymentsByOrderIdQuery request,
        CancellationToken cancellationToken)
    {
        using var connection = sqlConnectionFactory.CreateConnection();

        const string ownerSql = """
            SELECT customer_id
            FROM orders
            WHERE id = @OrderId
            """;

        var customerId = await connection.QueryFirstOrDefaultAsync<string>(
            new CommandDefinition(ownerSql, new { request.OrderId }, cancellationToken: cancellationToken));

        if (customerId is null)
            return Result.Failure<IReadOnlyList<PaymentResponse>>(OrderErrors.NotFound);

        if (!currentUser.IsInRole(Roles.Admin) && customerId != currentUser.UserId)
            return Result.Failure<IReadOnlyList<PaymentResponse>>(OrderErrors.Unauthorized);

        const string paymentsSql = """
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
            WHERE order_id = @OrderId
            ORDER BY created_at DESC
            """;

        var payments = await connection.QueryAsync<PaymentResponse>(
            new CommandDefinition(paymentsSql, new { request.OrderId }, cancellationToken: cancellationToken));

        return Result.Success<IReadOnlyList<PaymentResponse>>(payments.ToList());
    }
}
