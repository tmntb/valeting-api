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
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The cached or freshly generated value.</returns>
    Task<TResponse> GetOrCreateRecordAsync<TRequest, TResponse>(TRequest request, Func<Task<TResponse>> onCacheMiss, CacheOptions cacheOptions, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing cached record with new data. If the record doesn't exist, nothing is done.
    /// </summary>
    /// <param name="request">The request object to generate the hash key.</param>
    /// <param name="data">The new data to be updated in cache.</param>
    /// <param name="cacheOptions">Options to controller cache behaviour</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <typeparam name="TRequest">Type of the cache request key object.</typeparam>
    /// <typeparam name="TData">Type of the data object.</typeparam>
    /// <returns></returns>
    Task UpdateRecordAsync<TRequest, TData>(TRequest request, TData data, CacheOptions cacheOptions, CancellationToken cancellationToken = default);
    
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