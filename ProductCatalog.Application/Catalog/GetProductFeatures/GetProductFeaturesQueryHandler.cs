using Dapper;
using ProductCatalog.Application.Abstractions.Messaging;
using ProductCatalog.Application.Catalog.Responses;
using ProductCatalog.Application.Data;
using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Catalog.Errors;

namespace ProductCatalog.Application.Catalog.GetProductFeatures;

internal sealed class GetProductFeaturesQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    : IQueryHandler<GetProductFeaturesQuery, GetProductFeaturesQueryResponse>
{
    public async Task<Result<GetProductFeaturesQueryResponse>> Handle(
        GetProductFeaturesQuery request,
        CancellationToken cancellationToken)
    {
        using var connection = sqlConnectionFactory.CreateConnection();

        const string productSql = """
            SELECT id, name
            FROM products
            WHERE id = @ProductId
            """;

        var product = await connection.QueryFirstOrDefaultAsync<(Guid Id, string Name)>(
            new CommandDefinition(productSql, new { request.ProductId }, cancellationToken: cancellationToken));

        if (product == default)
            return Result.Failure<GetProductFeaturesQueryResponse>(ProductErrors.NotFound);

        const string featuresSql = """
            SELECT id, name, value, display_order
            FROM product_features
            WHERE product_id = @ProductId
            ORDER BY display_order
            """;

        var features = await connection.QueryAsync<ProductFeatureResponse>(
            new CommandDefinition(featuresSql, new { request.ProductId }, cancellationToken: cancellationToken));

        return Result.Success(new GetProductFeaturesQueryResponse(
            product.Id,
            product.Name,
            features.ToList()));
    }
}
