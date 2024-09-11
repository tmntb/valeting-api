using Microsoft.Extensions.Caching.Memory;

using Newtonsoft.Json;

using Valeting.Cache.Interfaces;

namespace Valeting.Cache;

public class MemoryCacheHandler(IMemoryCache memoryCache, ILogger<MemoryCacheHandler> logger) : ICacheHandler
{
    private const string CacheKeyListKey = "_cachedKeys";
    private readonly List<string> _cachedKeys = memoryCache.GetOrCreate(CacheKeyListKey, entry => new List<string>());

    private void AddKeyToCache(string recordKey)
    {
        if (!_cachedKeys.Contains(recordKey))
        {
            _cachedKeys.Add(recordKey);
            UpdateCachedKeysInCache();
        }
    }

    private void UpdateCachedKeysInCache()
    {
        memoryCache.Set(CacheKeyListKey, _cachedKeys);
    }

    public T? GetRecord<T>(string recordKey)
    {
        try
        {
            if (!memoryCache.TryGetValue(recordKey, out string serializedValue))
                return default;
            
            AddKeyToCache(recordKey);
            return JsonConvert.DeserializeObject<T>(serializedValue);

        } 
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving record with key {RecordKey}", recordKey);
        }

        return default;
    }

    public void RemoveRecordsWithPrefix(string prefix)
    {
        var keysToRemove = _cachedKeys.Where(k => k.StartsWith(prefix)).ToList();

        foreach (var key in keysToRemove)
        {
            memoryCache.Remove(key);
            _cachedKeys.Remove(key);
        }

        UpdateCachedKeysInCache();
    }

    public void RemoveRecord(string recordKey)
    {
        memoryCache.Remove(recordKey);
        _cachedKeys.Remove(recordKey);
        UpdateCachedKeysInCache();
    }

    public void SetRecord<T>(string recordKey, T data, TimeSpan? absoluteExpireTime = null, TimeSpan? slidingExpireTime = null)
    {
        try
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = absoluteExpireTime ?? TimeSpan.FromMinutes(1),
                SlidingExpiration = slidingExpireTime
            };

            var jsonData = JsonConvert.SerializeObject(data);
            memoryCache.Set(recordKey, jsonData, cacheEntryOptions);

            AddKeyToCache(recordKey);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error setting record with key {RecordKey}", recordKey);
        }
    }
}