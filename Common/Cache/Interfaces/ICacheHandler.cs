namespace Common.Cache.Interfaces;

public interface ICacheHandler
{
    Task<TResponse> GetOrCreateRecordAsync<TRequest, TResponse>(TRequest request, Func<Task<TResponse>> onCacheMiss, CacheOptions cacheOptions);
    void InvalidateAllCache();
    void InvalidateCacheById(Guid id);
    void InvalidateCacheByListType(CacheListType listType);
}