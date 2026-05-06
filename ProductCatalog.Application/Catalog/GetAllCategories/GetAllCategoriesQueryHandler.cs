using Microsoft.EntityFrameworkCore;
using ProductCatalog.Application.Abstractions.Messaging;
using ProductCatalog.Application.Catalog.Responses;
using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Catalog.Repositories;

namespace ProductCatalog.Application.Catalog.GetAllCategories;

internal sealed class GetAllCategoriesQueryHandler(IProductCategoryRepository productCategoryRepository)
    : IQueryHandler<GetAllCategoriesQuery, IReadOnlyList<ProductCategoryResponse>>
{
    public async Task<Result<IReadOnlyList<ProductCategoryResponse>>> Handle(
        GetAllCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var categories = await productCategoryRepository
            .Query()
            .Select(c => new ProductCategoryResponse(c.Id, c.Name, c.Description))
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyList<ProductCategoryResponse>>(categories);
    }
}