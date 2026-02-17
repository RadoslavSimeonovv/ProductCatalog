using ProductCatalog.Application.Catalog.Responses;
using ProductCatalog.Application.Messaging;
using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Catalog.Errors;
using ProductCatalog.Domain.Catalog.Repositories;

namespace ProductCatalog.Application.Catalog.GetProductFeatures;

internal sealed class GetProductFeaturesQueryHandler
    : IQueryHandler<GetProductFeaturesQuery, GetProductFeaturesQueryResponse>
{
    private readonly IProductRepository _productRepository;
    public GetProductFeaturesQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }
    public async Task<Result<GetProductFeaturesQueryResponse>> Handle(GetProductFeaturesQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product is null)
        {
            return Result.Failure<GetProductFeaturesQueryResponse>(ProductErrors.NotFound);
        }

        var response = new GetProductFeaturesQueryResponse
        {
            ProductId = product.Id,
            ProductName = product.Name,
            Features = product.Features
                .OrderBy(f => f.DisplayOrder)
                .Select(f => new ProductFeatureResponse
                {
                    Id = f.Id,
                    Name = f.Name,
                    Value = f.Value,
                    DisplayOrder = f.DisplayOrder
                })
                .ToList()
        };

        return Result.Success(response);
    }
}