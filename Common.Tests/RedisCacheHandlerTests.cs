using System.Text;
using System.Text.Json;
using Common.Cache;
using Microsoft.Extensions.Caching.Distributed;
using Moq;

namespace Common.Tests;

public class RedisCacheHandlerTests
{
    private readonly Mock<IDistributedCache> _distributedCache = new();
    private readonly RedisCacheHandler _cacheHandler;

    public RedisCacheHandlerTests()
    {
        _cacheHandler = new RedisCacheHandler(_distributedCache.Object);
    }

    [Fact]
    public async Task GetOrCreateRecordAsync_CacheMiss_CallsFactoryAndStoresValue()
    {
        var request = new CacheTestData(Guid.NewGuid(), "request");
        var expected = new CacheTestData(Guid.NewGuid(), "response");
        var options = new CacheOptions
        {
            AbsoluteExpireTime = TimeSpan.FromMinutes(5),
            SlidingExpireTime = TimeSpan.FromMinutes(1)
        };
        var factoryCalls = 0;
        SetupGet(null);
        SetupSet();

        var result = await _cacheHandler.GetOrCreateRecordAsync(request, () =>
        {
            factoryCalls++;
            return Task.FromResult(expected);
        }, options);

        Assert.Equal(expected, result);
        Assert.Equal(1, factoryCalls);
        _distributedCache.Verify(cache => cache.SetAsync(
            It.IsAny<string>(),
            It.Is<byte[]>(value => Encoding.UTF8.GetString(value) == JsonSerializer.Serialize(expected)),
            It.IsAny<DistributedCacheEntryOptions>(),
            It.IsAny<CancellationToken>()), Times.Once);

        var writtenOptions = GetWrittenOptions();
        Assert.Equal(options.AbsoluteExpireTime, writtenOptions.AbsoluteExpirationRelativeToNow);
        Assert.Equal(options.SlidingExpireTime, writtenOptions.SlidingExpiration);
    }

    [Fact]
    public async Task GetOrCreateRecordAsync_CacheHit_ReturnsDeserializedValueWithoutCallingFactory()
    {
        var request = new CacheTestData(Guid.NewGuid(), "request");
        var expected = new CacheTestData(Guid.NewGuid(), "cached");
        SetupGet(JsonSerializer.Serialize(expected));
        var factoryCalls = 0;

        var result = await _cacheHandler.GetOrCreateRecordAsync(request, () =>
        {
            factoryCalls++;
            return Task.FromResult(new CacheTestData(Guid.NewGuid(), "miss"));
        }, new CacheOptions());

        Assert.Equal(expected, result);
        Assert.Equal(0, factoryCalls);
        _distributedCache.Verify(cache => cache.SetAsync(
            It.IsAny<string>(),
            It.IsAny<byte[]>(),
            It.IsAny<DistributedCacheEntryOptions>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetOrCreateRecordAsync_WithoutAbsoluteExpiration_UsesDefaultExpiration()
    {
        SetupGet(null);
        SetupSet();

        await _cacheHandler.GetOrCreateRecordAsync(
            new CacheTestData(Guid.NewGuid(), "request"),
            () => Task.FromResult(new CacheTestData(Guid.NewGuid(), "response")),
            new CacheOptions());

        Assert.Equal(TimeSpan.FromSeconds(60), GetWrittenOptions().AbsoluteExpirationRelativeToNow);
    }

    [Fact]
    public async Task GetOrCreateRecordAsync_PassesCancellationTokenToDistributedCache()
    {
        using var cancellationSource = new CancellationTokenSource();
        var cancellationToken = cancellationSource.Token;
        SetupGet(null);
        SetupSet();

        await _cacheHandler.GetOrCreateRecordAsync(
            new CacheTestData(Guid.NewGuid(), "request"),
            () => Task.FromResult(new CacheTestData(Guid.NewGuid(), "response")),
            new CacheOptions(),
            cancellationToken);

        _distributedCache.Verify(cache => cache.GetAsync(It.IsAny<string>(), cancellationToken), Times.Once);
        _distributedCache.Verify(cache => cache.SetAsync(
            It.IsAny<string>(),
            It.IsAny<byte[]>(),
            It.IsAny<DistributedCacheEntryOptions>(),
            cancellationToken), Times.Once);
    }

    [Fact]
    public async Task GetOrCreateRecordAsync_NullArguments_ThrowsArgumentNullException()
    {
        var request = new CacheTestData(Guid.NewGuid(), "request");
        Func<Task<CacheTestData>> factory = () => Task.FromResult(request);

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _cacheHandler.GetOrCreateRecordAsync<CacheTestData?, CacheTestData>(null, factory, new CacheOptions()));
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _cacheHandler.GetOrCreateRecordAsync<CacheTestData, CacheTestData>(request, null!, new CacheOptions()));
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _cacheHandler.GetOrCreateRecordAsync(request, factory, null!));

        _distributedCache.Verify(cache => cache.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetOrCreateRecordAsync_FactoryThrows_PropagatesExceptionWithoutCaching()
    {
        var expectedException = new InvalidOperationException("Data source failed.");
        SetupGet(null);

        var actualException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _cacheHandler.GetOrCreateRecordAsync(
                new CacheTestData(Guid.NewGuid(), "request"),
                () => Task.FromException<CacheTestData>(expectedException),
                new CacheOptions()));

        Assert.Same(expectedException, actualException);
        _distributedCache.Verify(cache => cache.SetAsync(
            It.IsAny<string>(),
            It.IsAny<byte[]>(),
            It.IsAny<DistributedCacheEntryOptions>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateRecordAsync_ExistingValue_StoresUpdatedDataWithAbsoluteExpiration()
    {
        var request = new CacheTestData(Guid.NewGuid(), "request");
        var updated = new CacheTestData(Guid.NewGuid(), "updated");
        var options = new CacheOptions { AbsoluteExpireTime = TimeSpan.FromMinutes(3) };
        SetupGet(JsonSerializer.Serialize(request));
        SetupSet();

        await _cacheHandler.UpdateRecordAsync(request, updated, options);

        _distributedCache.Verify(cache => cache.SetAsync(
            It.IsAny<string>(),
            It.Is<byte[]>(value => Encoding.UTF8.GetString(value) == JsonSerializer.Serialize(updated)),
            It.IsAny<DistributedCacheEntryOptions>(),
            It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(options.AbsoluteExpireTime, GetWrittenOptions().AbsoluteExpirationRelativeToNow);
    }

    [Fact]
    public async Task UpdateRecordAsync_MissingValue_DoesNotStoreData()
    {
        SetupGet(null);

        await _cacheHandler.UpdateRecordAsync(
            new CacheTestData(Guid.NewGuid(), "request"),
            new CacheTestData(Guid.NewGuid(), "updated"),
            new CacheOptions { AbsoluteExpireTime = TimeSpan.FromMinutes(3) });

        _distributedCache.Verify(cache => cache.SetAsync(
            It.IsAny<string>(),
            It.IsAny<byte[]>(),
            It.IsAny<DistributedCacheEntryOptions>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateRecordAsync_NullArgumentsOrExpiration_ThrowsArgumentNullException()
    {
        var request = new CacheTestData(Guid.NewGuid(), "request");
        var data = new CacheTestData(Guid.NewGuid(), "updated");
        var options = new CacheOptions { AbsoluteExpireTime = TimeSpan.FromMinutes(3) };

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _cacheHandler.UpdateRecordAsync<CacheTestData?, CacheTestData>(null, data, options));
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _cacheHandler.UpdateRecordAsync(request, (CacheTestData)null!, options));
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _cacheHandler.UpdateRecordAsync(request, data, null!));
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _cacheHandler.UpdateRecordAsync(request, data, new CacheOptions()));

        _distributedCache.Verify(cache => cache.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateRecordAsync_PassesCancellationTokenToDistributedCache()
    {
        using var cancellationSource = new CancellationTokenSource();
        var cancellationToken = cancellationSource.Token;
        SetupGet(JsonSerializer.Serialize(new CacheTestData(Guid.NewGuid(), "cached")));
        SetupSet();

        await _cacheHandler.UpdateRecordAsync(
            new CacheTestData(Guid.NewGuid(), "request"),
            new CacheTestData(Guid.NewGuid(), "updated"),
            new CacheOptions { AbsoluteExpireTime = TimeSpan.FromMinutes(3) },
            cancellationToken);

        _distributedCache.Verify(cache => cache.GetAsync(It.IsAny<string>(), cancellationToken), Times.Once);
        _distributedCache.Verify(cache => cache.SetAsync(
            It.IsAny<string>(),
            It.IsAny<byte[]>(),
            It.IsAny<DistributedCacheEntryOptions>(),
            cancellationToken), Times.Once);
    }

    [Fact]
    public async Task InvalidateCacheById_RegisteredId_RemovesItsCacheEntry()
    {
        var id = Guid.NewGuid();
        var cacheKey = await AddCacheEntry(new CacheOptions { Id = id });

        _cacheHandler.InvalidateCacheById(id);

        _distributedCache.Verify(cache => cache.Remove(cacheKey), Times.Once);
    }

    [Fact]
    public void InvalidateCacheById_UnregisteredId_DoesNotRemoveCacheEntry()
    {
        _cacheHandler.InvalidateCacheById(Guid.NewGuid());

        _distributedCache.Verify(cache => cache.Remove(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task InvalidateCacheByListType_RegisteredType_RemovesItsCacheEntry()
    {
        const CacheListType listType = CacheListType.Booking;
        var cacheKey = await AddCacheEntry(new CacheOptions { ListType = listType });

        _cacheHandler.InvalidateCacheByListType(listType);

        _distributedCache.Verify(cache => cache.Remove(cacheKey), Times.Once);
    }

    [Fact]
    public void InvalidateCacheByListType_UnregisteredType_DoesNotRemoveCacheEntry()
    {
        _cacheHandler.InvalidateCacheByListType(CacheListType.VehicleSize);

        _distributedCache.Verify(cache => cache.Remove(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task InvalidateAllCache_RemovesEntriesRegisteredByIdAndListType()
    {
        var idCacheKey = await AddCacheEntry(new CacheOptions { Id = Guid.NewGuid() });
        var listCacheKey = await AddCacheEntry(new CacheOptions { ListType = CacheListType.Flexibility });

        _cacheHandler.InvalidateAllCache();

        _distributedCache.Verify(cache => cache.Remove(idCacheKey), Times.Once);
        _distributedCache.Verify(cache => cache.Remove(listCacheKey), Times.Once);
    }

    private async Task<string> AddCacheEntry(CacheOptions options)
    {
        SetupGet(null);
        SetupSet();
        string? cacheKey = null;
        _distributedCache
            .Setup(cache => cache.SetAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()))
            .Callback<string, byte[], DistributedCacheEntryOptions, CancellationToken>(
                (key, _, _, _) => cacheKey = key)
            .Returns(Task.CompletedTask);

        await _cacheHandler.GetOrCreateRecordAsync(
            new CacheTestData(Guid.NewGuid(), Guid.NewGuid().ToString()),
            () => Task.FromResult(new CacheTestData(Guid.NewGuid(), "cached")),
            options);

        return Assert.IsType<string>(cacheKey);
    }

    private void SetupGet(string? value)
    {
        var bytes = value is null ? null : Encoding.UTF8.GetBytes(value);
        _distributedCache
            .Setup(cache => cache.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(bytes);
    }

    private void SetupSet()
    {
        _distributedCache
            .Setup(cache => cache.SetAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    }

    private DistributedCacheEntryOptions GetWrittenOptions()
    {
        var setInvocation = Assert.Single(
            _distributedCache.Invocations,
            invocation => invocation.Method.Name == nameof(IDistributedCache.SetAsync));
        return Assert.IsType<DistributedCacheEntryOptions>(setInvocation.Arguments[2]);
    }

    private sealed record CacheTestData(Guid Id, string Name);
}
