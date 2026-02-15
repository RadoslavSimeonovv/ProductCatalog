using ProductCatalog.Application.Catalog.PaginatedResponse;
using ProductCatalog.Application.Catalog.Responses;
using ProductCatalog.Application.Messaging;
using ProductCatalog.Domain.Catalog.Enums;

namespace ProductCatalog.Application.Catalog.GetProductsQuery;

public sealed record GetProductsQuery(
    int PageNumber = 1,
    int PageSize = 10,
    ProductStatus? ProductStatus = null,
    Guid? CategoryId = null,
    ProductSortBy SortBy = ProductSortBy.Name,
    string? SearchTerm = null)
    : IQuery<PagedResult<ProductResponse>>;