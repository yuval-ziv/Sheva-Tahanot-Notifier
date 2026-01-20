using Microsoft.Extensions.Caching.Hybrid;

namespace ShevaTahanotNotifier.Services;

public class CacheService : ICacheService
{
    private readonly HybridCache _cache;

    public CacheService(HybridCache cache)
    {
        _cache = cache;
    }

    public ValueTask<T> GetOrCreateAsync<T>(string key, Func<CancellationToken, ValueTask<T>> factory, CancellationToken cancellationToken = default)
    {
        var cacheOptions = new HybridCacheEntryOptions
        {
            Expiration = TimeSpan.FromMinutes(15),
            LocalCacheExpiration = TimeSpan.FromMinutes(15),
        };
        return _cache.GetOrCreateAsync(key, factory, options: cacheOptions, cancellationToken: cancellationToken);
    }

    public ValueTask SetAsync<T>(string key, T value, CancellationToken cancellationToken = default)
    {
        var cacheOptions = new HybridCacheEntryOptions
        {
            Expiration = TimeSpan.FromMinutes(15),
            LocalCacheExpiration = TimeSpan.FromMinutes(15),
        };
        return _cache.SetAsync(key, value, options: cacheOptions, cancellationToken: cancellationToken);
    }
}