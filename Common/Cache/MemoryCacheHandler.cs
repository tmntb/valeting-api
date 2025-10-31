using Microsoft.Extensions.Caching.Memory;
using System.Text;
using System.Text.Json;
using System.Security.Cryptography;
using System.Collections.Concurrent;
using Common.Cache.Interfaces;

namespace Common.Cache;

public class MemoryCacheHandler(IMemoryCache memoryCache) : ICacheHandler
{
    private static readonly ConcurrentDictionary<Guid, string> _cachedKeys = new();
    private static readonly ConcurrentDictionary<CacheListType, string> _cachedListKeys = new();

    /// <inheritdoc />
    public async Task<TResponse> GetOrCreateRecordAsync<TRequest, TResponse>(TRequest request, Func<Task<TResponse>> onCacheMiss, CacheOptions cacheOptions)
    {
        var hashKey = GenerateHashKey(request);
        return await memoryCache.GetOrCreateAsync(hashKey, async entry =>
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = cacheOptions.AbsoluteExpireTime ?? TimeSpan.FromMinutes(1),
                    SlidingExpiration = cacheOptions.SlidingExpireTime
                };

                entry.SetOptions(cacheEntryOptions);

                if (cacheOptions.Id.HasValue)
                    _cachedKeys.TryAdd(cacheOptions.Id.Value, hashKey);

                if (cacheOptions.ListType.HasValue)
                    _cachedListKeys.TryAdd(cacheOptions.ListType.Value, hashKey);

                return await onCacheMiss();
            }
        );
    }

    /// <inheritdoc />
    public void InvalidateAllCache()
    {
        foreach (var key in _cachedKeys.Keys)
        {
            memoryCache.Remove(key);
        }

        foreach (var key in _cachedListKeys.Keys)
        {
            memoryCache.Remove(key);
        }

        _cachedKeys.Clear();
        _cachedListKeys.Clear();
    }

    /// <inheritdoc />
    public void InvalidateCacheById(Guid id)
    {
        if (_cachedKeys.TryRemove(id, out var key))
        {
            memoryCache.Remove(key);
        }
    }

    /// <inheritdoc />
    public void InvalidateCacheByListType(CacheListType listType)
    {
        if (_cachedListKeys.TryRemove(listType, out var key))
        {
            memoryCache.Remove(key);
        }
    }

    /// <summary>
    /// Generates a deterministic hash key from the request object for use in caching.
    /// </summary>
    /// <typeparam name="TRequest">Type of the request object.</typeparam>
    /// <param name="requestData">The object to hash.</param>
    /// <returns>A lowercase hexadecimal SHA-256 hash string representing the object.</returns>
    private static string GenerateHashKey<TRequest>(TRequest requestData)
    {
        var json = JsonSerializer.Serialize(requestData, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(json));
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }
}