using ProductCatalog.Application.Messaging;
using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Catalog.Entities;
using ProductCatalog.Domain.Catalog.Errors;
using ProductCatalog.Domain.Catalog.Repositories;
using ProductCatalog.Domain.Catalog.ValueObjects;

namespace ProductCatalog.Application.Catalog.CreateProduct;

internal sealed class CreateProductCommandHandler : ICommandHandler<CreateProductCommand, Guid>
{
    private readonly IProductRepository _productRepository;
    private readonly IProductCategoryRepository _productCategoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    public CreateProductCommandHandler(
        IProductRepository productRepository, 
        IProductCategoryRepository productCategoryRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _productCategoryRepository = productCategoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var category = await _productCategoryRepository.GetByIdAsync(request.CategoryId, cancellationToken); 

        if (category is null)
        {
            return Result.Failure<Guid>(ProductErrors.CategoryNotFound);
        }

        var product = Product.Create(
            request.Name,
            request.Description,
            request.Price,
            request.CategoryId,
            new Sku(request.Sku)
        );

        _productRepository.Add(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(product.Id);
    }
}