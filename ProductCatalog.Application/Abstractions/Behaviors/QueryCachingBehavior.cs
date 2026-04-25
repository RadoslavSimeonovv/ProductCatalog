using MediatR;
using Microsoft.Extensions.Logging;
using ProductCatalog.Application.Abstractions.Caching;
using ProductCatalog.Domain.Abstractions;

namespace ProductCatalog.Application.Abstractions.Behaviors;

public sealed class QueryCachingBehavior<TRequest, TResponse>(
    ICacheService cacheService,
    ILogger<QueryCachingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICachedQuery
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        TResponse? cached = await cacheService.GetAsync<TResponse>(request.CacheKey, cancellationToken);

        if (cached is not null)
        {
            logger.LogDebug("Cache hit for {CacheKey}", request.CacheKey);
            return cached;
        }

        TResponse result = await next();

        if (result is Result { IsSuccess: true })
            await cacheService.SetAsync(request.CacheKey, result, request.Expiration, cancellationToken);

        return result;
    }
}
