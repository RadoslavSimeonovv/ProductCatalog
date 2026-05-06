using Dapper;
using ProductCatalog.Application.Abstractions.Messaging;
using ProductCatalog.Application.Catalog.Responses;
using ProductCatalog.Application.Data;
using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Catalog.Errors;

namespace ProductCatalog.Application.Catalog.GetProductById;

internal sealed class GetProductByIdQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    : IQueryHandler<GetProductByIdQuery, ProductResponse>
{
    public async Task<Result<ProductResponse>> Handle(
        GetProductByIdQuery request,
        CancellationToken cancellationToken)
    {
        using var connection = sqlConnectionFactory.CreateConnection();

        const string sql = """
            SELECT
                id             AS Id,
                name           AS Name,
                description    AS Description,
                price_amount   AS Price,
                price_currency AS Currency,
                sku            AS Sku,
                status         AS Status,
                category_id    AS CategoryId
            FROM products
            WHERE id = @Id
            """;

        var product = await connection.QueryFirstOrDefaultAsync<ProductResponse>(
            new CommandDefinition(sql, new { request.Id }, cancellationToken: cancellationToken));

        if (product is null)
            return Result.Failure<ProductResponse>(ProductErrors.NotFound);

        return Result.Success(product);
    }
}
