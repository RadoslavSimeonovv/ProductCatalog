using Dapper;
using ProductCatalog.Application.Abstractions.Messaging;
using ProductCatalog.Application.Catalog.Responses;
using ProductCatalog.Application.Data;
using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Catalog.Errors;

namespace ProductCatalog.Application.Catalog.GetCategoryById;

internal sealed class GetCategoryByIdQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    : IQueryHandler<GetCategoryByIdQuery, ProductCategoryResponse>
{
    public async Task<Result<ProductCategoryResponse>> Handle(
        GetCategoryByIdQuery request,
        CancellationToken cancellationToken)
    {
        using var connection = sqlConnectionFactory.CreateConnection();

        const string sql = """
            SELECT id, name, description
            FROM product_categories
            WHERE id = @Id
            """;

        var category = await connection.QueryFirstOrDefaultAsync<ProductCategoryResponse>(
            new CommandDefinition(sql, new { request.Id }, cancellationToken: cancellationToken));

        if (category is null)
            return Result.Failure<ProductCategoryResponse>(ProductErrors.CategoryNotFound);

        return Result.Success(category);
    }
}
