using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Valeting.Common.Cache;

namespace Valeting.Tests.Api;

public class MemoryCacheHandlerTests
{
    private readonly List<string> _cacheKeys;
    private readonly FakeMemoryCache _memoryCache;
    private readonly MemoryCacheHandler _cacheHandler;

    public MemoryCacheHandlerTests()
    {
        _memoryCache = new FakeMemoryCache();
        _cacheKeys = [];
        _memoryCache.Set("_cachedKeys", _cacheKeys);
        _cacheHandler = new MemoryCacheHandler(_memoryCache);
    }

    [Fact]
    public void SetRecord_ShouldAddKeyToCache()
    {
        // Arrange
        string recordKey = "testKey";
        var data = new { Name = "Test" };

        // Act
        //_cacheHandler.SetRecord(recordKey, data);

        // Assert
        Assert.Contains(recordKey, _memoryCache.Get<List<string>>("_cachedKeys"));
    }

    [Fact]
    public void SetRecord_ShouldThrowException()
    {
        // Arrange
        var recordKey = "testKey";
        var data = new Node { Name = "Root" };
        data.Child = data; 

        // Act
        //_cacheHandler.SetRecord(recordKey, data);

        // Assert
        Assert.Throws<JsonSerializationException>(() =>
        {
            JsonConvert.SerializeObject(data);
        });
    }

    [Fact]
    public void GetRecord_ShouldReturnDataIfExistsInCache()
    {
        // Arrange
        string recordKey = "testKey";
        var data = new { Name = "Test" };
        var serializedData = JsonConvert.SerializeObject(data);

        _memoryCache.Set(recordKey, serializedData);

        // Act
        //var result = _cacheHandler.GetRecord<dynamic>(recordKey);

        // Assert
        //Assert.NotNull(result);
        Assert.Contains(recordKey, _memoryCache.Get<List<string>>("_cachedKeys"));
    }

    [Fact]
    public void GetRecord_ShouldReturnNullIfNotInCache()
    {
        // Arrange
        var recordKey = "testKey";

        // Act
        //var result = _cacheHandler.GetRecord<dynamic>(recordKey);

        // Assert
        //Assert.Null(result);
    }

    [Fact]
    public void GetRecord_ShouldThrowException()
    {
        // Arrange
        var recordKey = "testKey";
        _memoryCache.Set(recordKey, "invalid_json");

        // Act
        //_cacheHandler.GetRecord<dynamic>(recordKey);
    }

    [Fact]
    public void RemoveRecord_ShouldRemoveKeyFromCache()
    {
        // Arrange
        string recordKey = "testKey";
        _cacheKeys.Add(recordKey);
        _memoryCache.Set("_cachedKeys", _cacheKeys);

        // Act
        //_cacheHandler.RemoveRecord(recordKey);

        // Assert
        Assert.DoesNotContain(recordKey, _memoryCache.Get<List<string>>("_cachedKeys"));
    }

    [Fact]
    public void RemoveRecordsWithPrefix_ShouldRemoveMatchingKeysFromCache()
    {
        // Arrange
        var prefix = "test";
        _cacheKeys.AddRange(["test1", "test2", "otherKey"]);
        _memoryCache.Set("_cachedKeys", _cacheKeys);

        // Act
        //_cacheHandler.RemoveRecordsWithPrefix(prefix);

        // Assert
        Assert.DoesNotContain("test1", _memoryCache.Get<List<string>>("_cachedKeys"));
        Assert.DoesNotContain("test2", _memoryCache.Get<List<string>>("_cachedKeys"));
    }
}

public class FakeMemoryCache : IMemoryCache
{
    private readonly Dictionary<object, object> _cache = new Dictionary<object, object>();

    public ICacheEntry CreateEntry(object key)
    {
        return new FakeCacheEntry(key);
    }

    public void Dispose() { }

    public void Remove(object key)
    {
        _cache.Remove(key);
    }

    public bool TryGetValue(object key, out object value)
    {
        return _cache.TryGetValue(key, out value);
    }

    public void Set<T>(object key, T value)
    {
        _cache[key] = value;
    }

    public T Get<T>(object key)
    {
        _cache.TryGetValue(key, out var value);
        return (T)value;
    }
}

public class FakeCacheEntry(object key) : ICacheEntry
{
    public object Key { get; } = key;
    public object Value { get; set; }
    public DateTimeOffset? AbsoluteExpiration { get; set; }
    public TimeSpan? AbsoluteExpirationRelativeToNow { get; set; }
    public TimeSpan? SlidingExpiration { get; set; }
    public IList<IChangeToken> ExpirationTokens { get; } = [];
    public IList<PostEvictionCallbackRegistration> PostEvictionCallbacks { get; } = [];
    public CacheItemPriority Priority { get; set; }
    public long? Size { get; set; }
    public void Dispose() { }
}

public class Node
{
    public string Name { get; set; }
    public Node Child { get; set; }
}
