using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Common.Cache.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace Common.Cache;

public class RedisCacheHandler(IDistributedCache cache) : ICacheHandler
{
    private static readonly ConcurrentDictionary<Guid, string> CachedKeys = new();
    private static readonly ConcurrentDictionary<CacheListType, string> CachedListKeys = new();

    /// <inheritdoc />
    public async Task<TResponse> GetOrCreateRecordAsync<TRequest, TResponse>(TRequest request, Func<Task<TResponse>> onCacheMiss, CacheOptions cacheOptions, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(onCacheMiss);
        ArgumentNullException.ThrowIfNull(cacheOptions);

        var hashKey = GenerateHashKey(request);
        var cachedData = await cache.GetStringAsync(hashKey, cancellationToken);
        if (cachedData is not null)
            return JsonSerializer.Deserialize<TResponse>(cachedData);

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = cacheOptions.AbsoluteExpireTime ?? TimeSpan.FromSeconds(60),
            SlidingExpiration = cacheOptions.SlidingExpireTime
        };

        var data = await onCacheMiss();
        var jsonData = JsonSerializer.Serialize(data);

        if (cacheOptions.Id.HasValue)
            CachedKeys.TryAdd(cacheOptions.Id.Value, hashKey);

        if (cacheOptions.ListType.HasValue)
            CachedListKeys.TryAdd(cacheOptions.ListType.Value, hashKey);

        await cache.SetStringAsync(hashKey, jsonData, options, cancellationToken);

        return JsonSerializer.Deserialize<TResponse>(jsonData);
    }

    /// <inheritdoc />
    public async Task UpdateRecordAsync<TRequest, TData>(TRequest request, TData data, CacheOptions cacheOptions, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(data);
        ArgumentNullException.ThrowIfNull(cacheOptions);
        ArgumentNullException.ThrowIfNull(cacheOptions.AbsoluteExpireTime);
        
        var hashKey = GenerateHashKey(request);
        var cachedData = await cache.GetStringAsync(hashKey, cancellationToken);

        if (cachedData is null) return;
        
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = cacheOptions.AbsoluteExpireTime,
        };

        var updatedJson = JsonSerializer.Serialize(data);
        await cache.SetStringAsync(hashKey, updatedJson, options, cancellationToken);
    }

    /// <inheritdoc />
    public void InvalidateAllCache()
    {
        foreach (var hashKey in CachedKeys.Values)
            cache.Remove(hashKey);

        foreach (var hashKey in CachedListKeys.Values)
            cache.Remove(hashKey);

        CachedKeys.Clear();
        CachedListKeys.Clear();
    }

    /// <inheritdoc />
    public void InvalidateCacheById(Guid id)
    {
        if (CachedKeys.TryRemove(id, out var hashKey))
            cache.Remove(hashKey);
    }

    /// <inheritdoc />
    public void InvalidateCacheByListType(CacheListType listType)
    {
        if (CachedListKeys.TryRemove(listType, out var hashKey))
            cache.Remove(hashKey);
    }

    /// <summary>
    /// Generates a deterministic hash key from the request object for use in caching.
    /// </summary>
    /// <typeparam name="TRequest">Type of the request object.</typeparam>
    /// <param name="requestData">The object to hash.</param>
    /// <returns>A lowercase hexadecimal SHA-256 hash string representing the object.</returns>
    private static string GenerateHashKey<TRequest>(TRequest requestData)
    {
        var json = JsonSerializer.Serialize(requestData,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(json));
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }
}