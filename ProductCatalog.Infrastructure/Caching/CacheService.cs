using Microsoft.Extensions.Caching.Distributed;
using ProductCatalog.Application.Abstractions.Caching;
using System.Text.Json;

namespace ProductCatalog.Infrastructure.Caching;

internal sealed class CacheService : ICacheService
{
    private readonly IDistributedCache _cache;

    public CacheService(IDistributedCache cache)
    {
        _cache = cache;
    }
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        byte[]? bytes = await _cache.GetAsync(key, cancellationToken);

        if (bytes is null)
            return default;

        try
        {
            return Deserialize<T>(bytes);
        }
        catch (JsonException)
        {
            await _cache.RemoveAsync(key, cancellationToken);
            return default;
        }
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        => _cache.RemoveAsync(key, cancellationToken);

    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        byte[] bytes = Serialize(value);

        return _cache.SetAsync(key, bytes, CacheOptions.Create(expiration), cancellationToken);
    }

    public async Task<T> GetOrSetAsync<T>(string key, Func<CancellationToken, Task<T>> factory, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        T? cached = await GetAsync<T>(key, cancellationToken);

        if (cached is not null)
            return cached;

        T value = await factory(cancellationToken);

        await SetAsync(key, value, expiration, cancellationToken);

        return value;
    }

    private static T? Deserialize<T>(byte[] bytes) => JsonSerializer.Deserialize<T>(bytes);

    private static byte[] Serialize<T>(T value) => JsonSerializer.SerializeToUtf8Bytes(value);
}
