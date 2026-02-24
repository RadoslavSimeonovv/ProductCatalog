using ProductCatalog.Application.Abstractions.Messaging;
using ProductCatalog.Application.Catalog.Responses;
using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Catalog.Errors;
using ProductCatalog.Domain.Catalog.Repositories;

namespace ProductCatalog.Application.Catalog.GetCategoryById;

internal sealed class GetCategoryByIdQueryHandler
    : IQueryHandler<GetCategoryByIdQuery, ProductCategoryResponse>
{
    private readonly IProductCategoryRepository _productCategoryRepository;
    public GetCategoryByIdQueryHandler(IProductCategoryRepository productCategoryRepository)
    {
        _productCategoryRepository = productCategoryRepository;
    }

    public async Task<Result<ProductCategoryResponse>> Handle(
        GetCategoryByIdQuery request, 
        CancellationToken cancellationToken)
    {
        var category = await _productCategoryRepository
            .GetByIdAsync(request.Id, cancellationToken);

        if (category is null)
            return Result.Failure<ProductCategoryResponse>(ProductErrors.CategoryNotFound);

        return Result.Success(new ProductCategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        });
    }
}
