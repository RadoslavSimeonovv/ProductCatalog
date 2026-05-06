using Dapper;
using ProductCatalog.Application.Abstractions.Messaging;
using ProductCatalog.Application.Data;
using ProductCatalog.Application.Order.Responses;
using ProductCatalog.Domain.Abstractions;

namespace ProductCatalog.Application.Order.GetAllOrders;

internal sealed class GetAllOrdersQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    : IQueryHandler<GetAllOrdersQuery, List<OrderResponse>>
{
    public async Task<Result<List<OrderResponse>>> Handle(
        GetAllOrdersQuery request,
        CancellationToken cancellationToken)
    {
        using var connection = sqlConnectionFactory.CreateConnection();

        const string sql = """
            SELECT
                o.id             AS Id,
                o.customer_id    AS CustomerId,
                o.customer_email AS CustomerEmail,
                o.status         AS Status,
                i.product_id     AS ProductId,
                i.quantity       AS Quantity,
                i.unit_price_amount    AS UnitPrice,
                i.unit_price_currency  AS Currency
            FROM orders o
            LEFT JOIN order_items i ON i.order_id = o.id
            ORDER BY o.created_at DESC
            """;

        var rows = (await connection.QueryAsync<OrderRow>(
            new CommandDefinition(sql, cancellationToken: cancellationToken))).ToList();

        var orders = rows
            .GroupBy(r => r.Id)
            .Select(g => new OrderResponse(
                g.Key,
                g.First().CustomerId,
                g.First().CustomerEmail,
                g.First().Status,
                g.Where(r => r.ProductId.HasValue)
                 .Select(r => new OrderItemResponse(
                     r.ProductId!.Value,
                     r.Quantity!.Value,
                     r.UnitPrice!.Value,
                     r.Currency!))
                 .ToList()))
            .ToList();

        return Result.Success(orders);
    }
}
