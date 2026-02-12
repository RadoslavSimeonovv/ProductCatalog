using ProductCatalog.Application.Messaging;
using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Catalog.Errors;
using ProductCatalog.Domain.Catalog.Repositories;

namespace ProductCatalog.Application.Catalog.UpdateProductFeature;

internal sealed class UpdateProductFeatureValueCommandHandler : ICommandHandler<UpdateProductFeatureValueCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly IProductFeatureRepository _productFeatureRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductFeatureValueCommandHandler(
        IProductRepository productRepository,
        IProductFeatureRepository productFeatureRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _productFeatureRepository = productFeatureRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateProductFeatureValueCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product is null)
            return Result.Failure(ProductErrors.NotFound);

        var result = product.UpdateFeatureValue(request.FeatureId, request.NewValue);

        if (result.IsFailure)
            return Result.Failure(result.Error);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
