using Microsoft.EntityFrameworkCore;
using ProductCatalog.Application.Catalog.Responses;
using ProductCatalog.Application.Messaging;
using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Catalog.Repositories;

namespace ProductCatalog.Application.Catalog.GetAllCategories;

internal sealed class GetAllCategoriesQueryHandler
    : IQueryHandler<GetAllCategoriesQuery, IReadOnlyList<ProductCategoryResponse>>
{
    private readonly IProductCategoryRepository _productCategoryRepository;
    public GetAllCategoriesQueryHandler(IProductCategoryRepository productCategoryRepository)
    {
        _productCategoryRepository = productCategoryRepository;
    }
    public async Task<Result<IReadOnlyList<ProductCategoryResponse>>> Handle(
        GetAllCategoriesQuery request, 
        CancellationToken cancellationToken)
    {
        var categories = await _productCategoryRepository
            .Query()
            .AsNoTracking()
            .Select(c => new ProductCategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            })
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyList<ProductCategoryResponse>>(categories);
    }
}