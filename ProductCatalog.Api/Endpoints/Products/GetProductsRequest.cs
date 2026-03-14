using ProductCatalog.Application.Catalog.GetProducts;
using ProductCatalog.Domain.Catalog.Enums;

namespace ProductCatalog.Api.Endpoints.Products;

public sealed record GetProductsRequest(
    int PageNumber = 1,
    int PageSize = 10,
    ProductStatus? ProductStatus = null,
    Guid? CategoryId = null,
    ProductSortBy SortBy = ProductSortBy.Name,
    string? SearchTerm = null);
