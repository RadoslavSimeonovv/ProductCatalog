using ProductCatalog.Application.Catalog.PaginatedResponse;
using ProductCatalog.Application.Catalog.Responses;
using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Catalog.Entities;
using ProductCatalog.Domain.Catalog.Repositories;
using Microsoft.EntityFrameworkCore;
using ProductCatalog.Application.Abstractions.Messaging;

namespace ProductCatalog.Application.Catalog.GetProducts;

internal sealed class GetProductsQueryHandler(IProductRepository productRepository)
    : IQueryHandler<GetProductsQuery, PagedResult<ProductResponse>>
{
    public async Task<Result<PagedResult<ProductResponse>>> Handle(
        GetProductsQuery request,
        CancellationToken cancellationToken)
    {
        var products = productRepository.QueryWithCategory();

        var page = request.PageNumber is < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize is < 1 ? 10 : request.PageSize;

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.Trim();
            products = products!.Where(p =>
            p.Name.Contains(searchTerm) ||
            (p.Description != null && p.Description.Contains(searchTerm)));
        }

        if (request.CategoryId is not null)
        {
            var categoryId = request.CategoryId.Value;
            products = products!.Where(p => p.CategoryId == categoryId);
        }

        if (request.ProductStatus is not null)
        {
            var productStatus = request.ProductStatus.Value;
            products = products!.Where(p => p.Status == productStatus);
        }

        var totalCount = await products.CountAsync();

        products = ApplySorting(products!, request.SortBy);

        // Paging + projection
        var items = await products
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProductResponse(
                p.Id,
                p.Name,
                p.Description,
                p.Price.Amount,
                p.Price.Currency.Code,
                p.Sku.Value,
                p.Status.ToString(),
                p.CategoryId))
            .ToListAsync(cancellationToken);

        return Result.Success(new PagedResult<ProductResponse>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        });
    }


    private static IQueryable<Product> ApplySorting(
       IQueryable<Product> products,
       ProductSortBy sortBy)
    {
        return sortBy switch
        {
            ProductSortBy.Name => products
                                    .OrderBy(p => p.Name),
            ProductSortBy.Category => products
                                    .OrderBy(p => p.Category!.Name),
            ProductSortBy.Price => products
                                    .OrderBy(p => p.Price.Amount),
            ProductSortBy.Sku => products
                                    .OrderBy(p => p.Sku.Value),

            _ => products.OrderBy(p => p.Name)
        };
    }
}