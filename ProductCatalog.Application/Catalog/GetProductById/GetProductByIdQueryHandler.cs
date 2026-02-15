using ProductCatalog.Application.Catalog.Responses;
using ProductCatalog.Application.Messaging;
using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Catalog.Errors;
using ProductCatalog.Domain.Catalog.Repositories;

namespace ProductCatalog.Application.Catalog.GetProductById;

internal sealed class GetProductByIdQueryHandler : IQueryHandler<GetProductByIdQuery, ProductResponse>
{
    private readonly IProductRepository _productRepository;
    public GetProductByIdQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }
    public async Task<Result<ProductResponse>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);

        if (product == null)
            return Result.Failure<ProductResponse>(ProductErrors.NotFound);

        var response = new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price.Amount,
            Currency = product.Price.Currency.Code,
            Sku = product.Sku.Value
        };
        
        return Result.Success(response);
    }
}
