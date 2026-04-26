using ProductCatalog.Application.Abstractions.Messaging;
using ProductCatalog.Application.Exceptions;
using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Catalog.Errors;
using ProductCatalog.Domain.Catalog.Repositories;
using ProductCatalog.Domain.Shared.ValueObjects;

namespace ProductCatalog.Application.Catalog.ChangeProductPrice;

internal sealed class ChangeProductPriceCommandHandler : ICommandHandler<ChangeProductPriceCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    public ChangeProductPriceCommandHandler(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ChangeProductPriceCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product is null)
            return Result.Failure(ProductErrors.NotFound);  

        var currencyResult = Currency.FromCode(request.Currency);
        if (currencyResult.IsFailure)
            return Result.Failure(currencyResult.Error);

        var price = new Money(request.PriceAmount, currencyResult.Value);

        var result = product.ChangePrice(price);

        if (result.IsFailure)
            return result;

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (ConcurrencyException)
        {
            return Result.Failure(ProductErrors.ConcurrencyConflict);
        }
    }
}