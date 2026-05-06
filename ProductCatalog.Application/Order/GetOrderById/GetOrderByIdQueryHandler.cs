using Dapper;
using ProductCatalog.Application.Abstractions.Authentication;
using ProductCatalog.Application.Abstractions.Messaging;
using ProductCatalog.Application.Data;
using ProductCatalog.Application.Order.Responses;
using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Order.Errors;

namespace ProductCatalog.Application.Order.GetOrderById;

internal sealed class GetOrderByIdQueryHandler(ISqlConnectionFactory sqlConnectionFactory, ICurrentUser currentUser)
    : IQueryHandler<GetOrderByIdQuery, OrderResponse>
{
    public async Task<Result<OrderResponse>> Handle(
        GetOrderByIdQuery request,
        CancellationToken cancellationToken)
    {
        using var connection = sqlConnectionFactory.CreateConnection();

        const string orderSql = """
            SELECT id, customer_id, customer_email, status
            FROM orders
            WHERE id = @Id
            """;

        var order = await connection.QueryFirstOrDefaultAsync<(Guid Id, string CustomerId, string CustomerEmail, string Status)>(
            new CommandDefinition(orderSql, new { request.Id }, cancellationToken: cancellationToken));

        if (order == default)
            return Result.Failure<OrderResponse>(OrderErrors.NotFound);

        if (!currentUser.IsInRole(Roles.Admin) && order.CustomerId != currentUser.UserId)
            return Result.Failure<OrderResponse>(OrderErrors.Unauthorized);

        const string itemsSql = """
            SELECT product_id, quantity, unit_price_amount AS UnitPrice, unit_price_currency AS Currency
            FROM order_items
            WHERE order_id = @Id
            """;

        var items = await connection.QueryAsync<OrderItemResponse>(
            new CommandDefinition(itemsSql, new { request.Id }, cancellationToken: cancellationToken));

        return Result.Success(new OrderResponse(
            order.Id,
            order.CustomerId,
            order.CustomerEmail,
            order.Status,
            items.ToList()));
    }
}
