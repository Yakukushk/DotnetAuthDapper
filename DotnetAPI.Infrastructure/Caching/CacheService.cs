
using DotnetAPI.Application.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Text.Json;

namespace DotnetAPI.Infrastructure.Caching;

public class CacheService : ICacheService
{
    private readonly IDistributedCache _distributedCache;
    private static ConcurrentDictionary<string, bool> _cacheKeys = new();
    public CacheService(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        string? cachedValue = await _distributedCache.GetStringAsync(key, cancellationToken);
        if (cachedValue == null)
        {
            return null;
        }

        T? value = JsonConvert.DeserializeObject<T>(cachedValue);
        return value;
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _distributedCache.RemoveAsync(key, cancellationToken);
        _cacheKeys.TryRemove(key, out _);
    }

    public async Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        var keys = _cacheKeys.Keys;
        foreach (var key in keys)
        {
            if (key.StartsWith(prefix))
            {
                await RemoveAsync(key, cancellationToken);
            }
        }
    }

    public async Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default) where T : class
    {
        string cacheValue = JsonConvert.SerializeObject(value);
        await _distributedCache.SetStringAsync(key, cacheValue, cancellationToken);
        _cacheKeys.TryAdd(key, false);
    }
}

