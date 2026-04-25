using MediatR;
using ProductCatalog.Application.Abstractions.Caching;
using ProductCatalog.Domain.Catalog.Events;

namespace ProductCatalog.Application.Catalog.CacheInvalidation;

internal sealed class ProductCacheInvalidationHandler(ICacheService cacheService)
    : INotificationHandler<ProductPriceChangedDomainEvent>,
      INotificationHandler<ProductActivatedDomainEvent>,
      INotificationHandler<ProductDeactivatedDomainEvent>,
      INotificationHandler<ProductDiscontinuedDomainEvent>,
      INotificationHandler<ProductCategoryChangedDomainEvent>
{
    public Task Handle(ProductPriceChangedDomainEvent notification, CancellationToken ct)
        => cacheService.RemoveAsync($"products:{notification.productId}", ct);

    public Task Handle(ProductActivatedDomainEvent notification, CancellationToken ct)
        => cacheService.RemoveAsync($"products:{notification.productId}", ct);

    public Task Handle(ProductDeactivatedDomainEvent notification, CancellationToken ct)
        => cacheService.RemoveAsync($"products:{notification.productId}", ct);

    public Task Handle(ProductDiscontinuedDomainEvent notification, CancellationToken ct)
        => cacheService.RemoveAsync($"products:{notification.productId}", ct);

    public Task Handle(ProductCategoryChangedDomainEvent notification, CancellationToken ct)
        => cacheService.RemoveAsync($"products:{notification.ProductId}", ct);
}

internal sealed class ProductFeatureCacheInvalidationHandler(ICacheService cacheService)
    : INotificationHandler<ProductFeatureAddedDomainEvent>,
      INotificationHandler<ProductFeatureRemovedDomainEvent>,
      INotificationHandler<ProductFeatureUpdatedDomainEvent>
{
    public Task Handle(ProductFeatureAddedDomainEvent notification, CancellationToken ct)
        => cacheService.RemoveAsync($"products:{notification.ProductId}:features", ct);

    public Task Handle(ProductFeatureRemovedDomainEvent notification, CancellationToken ct)
        => cacheService.RemoveAsync($"products:{notification.ProductId}:features", ct);

    public Task Handle(ProductFeatureUpdatedDomainEvent notification, CancellationToken ct)
        => cacheService.RemoveAsync($"products:{notification.ProductId}:features", ct);
}
