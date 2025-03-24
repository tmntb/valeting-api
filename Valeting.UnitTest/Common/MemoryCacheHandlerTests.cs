using Microsoft.Extensions.Caching.Memory;
using Moq;
using System.Text.Json;
using Valeting.Common.Cache;
using Valeting.Common.Models.Booking;

namespace Valeting.Tests.Common;

public class MemoryCacheHandlerTests
{
    private readonly Mock<IMemoryCache> _mockMemoryCache;

    private readonly Guid _mockId = Guid.Parse("00000000-0000-0000-0000-000000000000");
    private readonly MemoryCacheHandler _cacheHandler;

    public MemoryCacheHandlerTests()
    {
        _mockMemoryCache = new Mock<IMemoryCache>();

        _cacheHandler = new MemoryCacheHandler(_mockMemoryCache.Object);
    }

    [Fact]
    public async Task GetOrCreateRecordAsync_CacheMiss_CallsOnCacheMiss()
    {
        // Arrange
        var request = new { Id = _mockId, Value = "Test" };
        var cacheOptions = new CacheOptions { AbsoluteExpireTime = TimeSpan.FromMinutes(5), Id = request.Id };
        var expectedResponse = JsonSerializer.Serialize(request);

        SetupCacheMiss(expectedResponse);

        // Act
        var result = await _cacheHandler.GetOrCreateRecordAsync(request, () => SimulateCacheMiss(expectedResponse), cacheOptions);

        // Assert
        Assert.Equal(expectedResponse, result);
    }

    [Fact]
    public async Task GetOrCreateRecordAsync_CacheMissList_CallsOnCacheMiss()
    {
        // Arrange
        var request = new List<BookingDto> { new() };
        var cacheOptions = new CacheOptions { AbsoluteExpireTime = TimeSpan.FromMinutes(5), ListType = CacheListType.Booking };
        var expectedResponse = JsonSerializer.Serialize(request);

        SetupCacheMiss(expectedResponse);

        // Act
        var result = await _cacheHandler.GetOrCreateRecordAsync(request, () => SimulateCacheMiss(expectedResponse), cacheOptions);

        // Assert
        Assert.Equal(expectedResponse, result);
    }

    [Fact]
    public async Task GetOrCreateRecordAsync_CacheHit_ReturnsCachedValue()
    {
        // Arrange
        var request = new { Id = _mockId, Name = "Test" };
        var cacheOptions = new CacheOptions { AbsoluteExpireTime = TimeSpan.FromMinutes(5), Id = request.Id };
        var expectedResponse = JsonSerializer.Serialize(request);

        object cachedValue = expectedResponse;
        _mockMemoryCache.Setup(m => m.TryGetValue(It.IsAny<object>(), out cachedValue)).Returns(true);

        // Act
        var result = await _cacheHandler.GetOrCreateRecordAsync(request, () => SimulateCacheMiss(expectedResponse), cacheOptions);

        // Assert
        Assert.Equal(expectedResponse, result);
    }

    [Fact]
    public void InvalidateCacheById_RemovesItemFromCache()
    {
        // Arrange
        _mockMemoryCache.Setup(m => m.Remove(It.IsAny<object>()));

        // Act
        _cacheHandler.InvalidateCacheById(_mockId);

        // Assert
        _mockMemoryCache.Verify(m => m.Remove(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task InvalidateCacheByListType_RemovesItemFromCache()
    {
        // Arrange
        var listType = CacheListType.Booking;
        var request = new List<BookingDto> { new() };
        var cacheOptions = new CacheOptions { AbsoluteExpireTime = TimeSpan.FromMinutes(5), ListType = CacheListType.Booking };
        var expectedResponse = JsonSerializer.Serialize(request);

        SetupCacheMiss(expectedResponse);
        await _cacheHandler.GetOrCreateRecordAsync(request, () => SimulateCacheMiss(expectedResponse), cacheOptions);

        _mockMemoryCache.Setup(m => m.Remove(It.IsAny<object>()));

        // Act
        _cacheHandler.InvalidateCacheByListType(listType);

        // Assert
        _mockMemoryCache.Verify(m => m.Remove(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task InvalidateAllCache_RemovesAllItemsFromCache()
    {
        // Arrange
        var request = new { Id = _mockId, Value = "Test" };
        var cacheOptions = new CacheOptions { AbsoluteExpireTime = TimeSpan.FromMinutes(5), Id= request.Id };
        var expectedResponse = JsonSerializer.Serialize(request);

        SetupCacheMiss(expectedResponse);
        await _cacheHandler.GetOrCreateRecordAsync(request, () => SimulateCacheMiss(expectedResponse), cacheOptions);

        var requestList = new List<BookingDto> { new() };
        var cacheOptionsList = new CacheOptions { AbsoluteExpireTime = TimeSpan.FromMinutes(5), ListType = CacheListType.Booking };
        var expectedResponseList = JsonSerializer.Serialize(request);

        SetupCacheMiss(expectedResponseList);
        await _cacheHandler.GetOrCreateRecordAsync(requestList, () => SimulateCacheMiss(expectedResponseList), cacheOptionsList);

        _mockMemoryCache.Setup(m => m.Remove(It.IsAny<object>()));

        // Act
        _cacheHandler.InvalidateAllCache();

        // Assert
        _mockMemoryCache.Verify(m => m.Remove(It.IsAny<object>()), Times.AtLeastOnce);
    }

    private async Task<string> SimulateCacheMiss(string response)
    {
        return await Task.FromResult(response);
    }

    private void SetupCacheMiss(string expectedResponse)
    {
        object cachedValue = null;
        _mockMemoryCache.Setup(m => m.TryGetValue(It.IsAny<object>(), out cachedValue)).Returns(false);
        _mockMemoryCache.Setup(m => m.CreateEntry(It.IsAny<object>()))
            .Returns((object key) =>
            {
                var cacheEntry = new Mock<ICacheEntry>();
                cacheEntry.SetupAllProperties();
                cacheEntry.Object.Value = expectedResponse;
                return cacheEntry.Object;
            });
    }
}