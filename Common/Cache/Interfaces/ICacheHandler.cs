namespace Common.Cache.Interfaces;

public interface ICacheHandler
{
    /// <summary>
    /// Retrieves a cached record if available, otherwise executes the provided factory function and caches the result.
    /// </summary>
    /// <typeparam name="TRequest">Type of the cache request key object.</typeparam>
    /// <typeparam name="TResponse">Type of the cached value.</typeparam>
    /// <param name="request">The request object used to generate the cache key.</param>
    /// <param name="onCacheMiss">Function to execute if the record is not present in cache.</param>
    /// <param name="cacheOptions">Options for controlling caching behavior (expiration, list type, id).</param>
    /// <returns>The cached or freshly generated value.</returns>
    Task<TResponse> GetOrCreateRecordAsync<TRequest, TResponse>(TRequest request, Func<Task<TResponse>> onCacheMiss, CacheOptions cacheOptions);

    /// <summary>
    /// Clears all cached entries from memory.
    /// </summary>
    void InvalidateAllCache();

    /// <summary>
    /// Removes a cached entry corresponding to a specific entity ID.
    /// </summary>
    /// <param name="id">The ID of the entity to invalidate in cache.</param>
    void InvalidateCacheById(Guid id);

    /// <summary>
    /// Removes a cached entry corresponding to a specific entity list type.
    /// </summary>
    /// <param name="listType">The type of entity list to invalidate in cache.</param>
    void InvalidateCacheByListType(CacheListType listType);
}