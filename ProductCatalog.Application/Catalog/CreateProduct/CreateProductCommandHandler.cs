using ProductCatalog.Application.Abstractions.Messaging;
using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Catalog.Entities;
using ProductCatalog.Domain.Catalog.Errors;
using ProductCatalog.Domain.Catalog.Repositories;
using ProductCatalog.Domain.Catalog.ValueObjects;
using ProductCatalog.Domain.Shared.ValueObjects;

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
            return Result.Failure<Guid>(ProductErrors.CategoryNotFound);

        var skuResult = Sku.Create(request.Sku);

        if (skuResult.IsFailure)
            return Result.Failure<Guid>(skuResult.Error);

        var currencyResult = Currency.FromCode(request.Currency);
        if (currencyResult.IsFailure)
            return Result.Failure<Guid>(currencyResult.Error);

        var price = new Money(request.PriceAmount, currencyResult.Value);

        var result = Product.Create(
            request.Name,
            request.Description!,
            price,
            request.CategoryId,
            skuResult.Value
        );

        if (result.IsFailure)
            return Result.Failure<Guid>(result.Error);

        _productRepository.Add(result.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(result.Value.Id);
    }
}