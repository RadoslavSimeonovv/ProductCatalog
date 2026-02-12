using ProductCatalog.Application.Messaging;
using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Catalog.Errors;
using ProductCatalog.Domain.Catalog.Repositories;

namespace ProductCatalog.Application.Catalog.AddProductFeature;

internal sealed class AddProductFeatureCommandHandler : ICommandHandler<AddProductFeatureCommand, Guid>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddProductFeatureCommandHandler(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(AddProductFeatureCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product is null)
            return Result.Failure<Guid>(ProductErrors.NotFound);

        var result = product.AddFeature(request.FeatureId, request.Name, request.Value, request.DisplayOrder);

        if (result.IsFailure)
            return Result.Failure<Guid>(result.Error);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(request.FeatureId);
    }
}