using ProductCatalog.Application.Abstractions.Caching;
using ProductCatalog.Application.Catalog.PaginatedResponse;
using ProductCatalog.Application.Catalog.Responses;
using ProductCatalog.Domain.Catalog.Enums;

namespace ProductCatalog.Application.Catalog.GetProducts;

public sealed record GetProductsQuery(
    int PageNumber = 1,
    int PageSize = 10,
    ProductStatus? ProductStatus = null,
    Guid? CategoryId = null,
    ProductSortBy SortBy = ProductSortBy.Name,
    string? SearchTerm = null)
    : ICachedQuery<PagedResult<ProductResponse>>
{
    public string CacheKey =>
        $"products:list:{PageNumber}:{PageSize}:{ProductStatus}:{CategoryId}:{SortBy}:{SearchTerm}";

    public TimeSpan? Expiration => TimeSpan.FromMinutes(2);
}
