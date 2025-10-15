using Common.Cache;
using Common.Cache.Interfaces;
using Common.Messages;
using Moq;
using Service.Interfaces;
using Service.Models.Flexibility;
using Service.Models.Flexibility.Payload;
using Service.Services;

namespace Service.Tests.Services;

public class FlexibilityServiceTests
{
    private readonly Mock<IFlexibilityRepository> _mockFlexibilityRepository;
    private readonly Mock<ICacheHandler> _mockCacheHandler;

    private readonly Guid _mockId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private readonly FlexibilityService _flexibilityService;

    public FlexibilityServiceTests()
    {
        _mockFlexibilityRepository = new Mock<IFlexibilityRepository>();
        _mockCacheHandler = new Mock<ICacheHandler>();

        _flexibilityService = new FlexibilityService(_mockFlexibilityRepository.Object, _mockCacheHandler.Object);
    }

    [Fact]
    public async Task GetFilteredAsync_ShouldReturnCachedData_WhenAvailable()
    {
        // Arrange
        _mockCacheHandler.Setup(cache => cache.GetOrCreateRecordAsync(It.IsAny<FlexibilityFilterDto>(), It.IsAny<Func<Task<FlexibilityPaginatedDtoResponse>>>(), It.IsAny<CacheOptions>()))
            .ReturnsAsync(
                new FlexibilityPaginatedDtoResponse
                {
                    Flexibilities =
                    [
                        new() { Id = _mockId },
                        new() { Id = _mockId }
                    ],
                    TotalItems = 2,
                    TotalPages = 1
                });

        // Act
        var result = await _flexibilityService.GetFilteredAsync(
            new()
            {
                PageNumber = 1,
                PageSize = 2
            });

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalItems);
        Assert.Equal(1, result.TotalPages);
        _mockFlexibilityRepository.Verify(x => x.GetFilteredAsync(It.IsAny<FlexibilityFilterDto>()), Times.Never);
    }

    [Fact]
    public async Task GetFilteredAsync_ShouldThrowKeyNotFoundException_WhenNoFlexibilitiesFound()
    {
        // Arrange
        _mockCacheHandler.Setup(x => x.GetOrCreateRecordAsync(It.IsAny<FlexibilityFilterDto>(), It.IsAny<Func<Task<FlexibilityPaginatedDtoResponse>>>(), It.IsAny<CacheOptions>()))
            .Returns((FlexibilityFilterDto _, Func<Task<FlexibilityPaginatedDtoResponse>> factory, CacheOptions __) => factory());

        _mockFlexibilityRepository.Setup(repo => repo.GetFilteredAsync(It.IsAny<FlexibilityFilterDto>()))
            .ReturnsAsync(new List<FlexibilityDto>());

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _flexibilityService.GetFilteredAsync(
            new()
            {
                PageNumber = 1,
                PageSize = 2
            }));

        Assert.Equal(exception.Message, Messages.NotFound);
    }

    [Fact]
    public async Task GetFilteredAsync_ShouldReturnPaginatedData_WhenCacheMissAndDataExists()
    {
        // Arrange
        _mockCacheHandler.Setup(x => x.GetOrCreateRecordAsync(It.IsAny<FlexibilityFilterDto>(), It.IsAny<Func<Task<FlexibilityPaginatedDtoResponse>>>(), It.IsAny<CacheOptions>()))
             .Returns((FlexibilityFilterDto _, Func<Task<FlexibilityPaginatedDtoResponse>> factory, CacheOptions __) => factory());

        _mockFlexibilityRepository.Setup(x => x.GetFilteredAsync(It.IsAny<FlexibilityFilterDto>()))
            .ReturnsAsync(
                [
                    new() { Id = _mockId },
                    new() { Id = _mockId },
                    new() { Id = _mockId }
                ]);

        // Act
        var result = await _flexibilityService.GetFilteredAsync(
            new()
            {
                PageNumber = 1,
                PageSize = 2
            });

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Flexibilities.Count);
        Assert.Equal(3, result.TotalItems);
        Assert.Equal(2, result.TotalPages);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCachedData()
    {
        // Arrange
        _mockCacheHandler.Setup(c => c.GetOrCreateRecordAsync(It.IsAny<Guid>(), It.IsAny<Func<Task<FlexibilityDto>>>(), It.IsAny<CacheOptions>()))
            .ReturnsAsync(
                new FlexibilityDto
                {
                    Id = _mockId
                });

        // Act
        var result = await _flexibilityService.GetByIdAsync(_mockId);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result);
        Assert.Equal(_mockId, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowKeyNotFoundException_WhenNoFlexibilityFound()
    {
        // Arrange
        _mockCacheHandler.Setup(x => x.GetOrCreateRecordAsync(It.IsAny<Guid>(), It.IsAny<Func<Task<FlexibilityDto>>>(), It.IsAny<CacheOptions>()))
            .Returns((Guid _, Func<Task<FlexibilityDto>> factory, CacheOptions __) => factory());

        _mockFlexibilityRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((FlexibilityDto)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _flexibilityService.GetByIdAsync(_mockId));

        Assert.Equal(exception.Message, Messages.NotFound);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnPaginatedData_WhenCacheMissAndDataExists()
    {
        // Arrange
        _mockCacheHandler.Setup(x => x.GetOrCreateRecordAsync(It.IsAny<Guid>(), It.IsAny<Func<Task<FlexibilityDto>>>(), It.IsAny<CacheOptions>()))
            .Returns((Guid _, Func<Task<FlexibilityDto>> factory, CacheOptions __) => factory());

        _mockFlexibilityRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
             .ReturnsAsync(
                new FlexibilityDto
                {
                    Id = _mockId
                });

        // Act
        var result = await _flexibilityService.GetByIdAsync(_mockId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(_mockId, result.Id);
    }
}
