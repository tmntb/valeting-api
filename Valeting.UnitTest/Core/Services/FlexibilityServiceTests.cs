using Moq;
using Valeting.Common.Cache;
using Valeting.Common.Cache.Interfaces;
using Valeting.Common.Messages;
using Valeting.Common.Models.Flexibility;
using Valeting.Core.Services;
using Valeting.Repository.Interfaces;

namespace Valeting.Tests.Core.Services;

public class FlexibilityServiceTests
{
    private readonly Mock<IFlexibilityRepository> _mockRepository;
    private readonly Mock<ICacheHandler> _mockCacheHandler;

    private readonly Guid _mockId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private readonly FlexibilityService _service;

    public FlexibilityServiceTests()
    {
        _mockRepository = new Mock<IFlexibilityRepository>();
        _mockCacheHandler = new Mock<ICacheHandler>();

        _service = new FlexibilityService(_mockRepository.Object, _mockCacheHandler.Object);
    }

    [Fact]
    public async Task GetFilteredAsync_ShouldReturnCachedData_WhenAvailable()
    {
        // Arrange
        var request = new PaginatedFlexibilityDtoRequest
        {
            Filter = new()
            {
                PageNumber = 1,
                PageSize = 2
            }
        };
        var flexibilities = new List<FlexibilityDto>()
        {
            new() { Id = _mockId },
            new() { Id = _mockId }
        };

        _mockCacheHandler.Setup(cache => cache.GetOrCreateRecordAsync(request, It.IsAny<Func<Task<PaginatedFlexibilityDtoResponse>>>(), It.IsAny<CacheOptions>()))
            .ReturnsAsync(
                new PaginatedFlexibilityDtoResponse
                {
                    Flexibilities = flexibilities,
                    TotalItems = 2,
                    TotalPages = 1
                });

        // Act
        var result = await _service.GetFilteredAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalItems);
        Assert.Equal(1, result.TotalPages);
        _mockRepository.Verify(x => x.GetFilteredAsync(It.IsAny<FlexibilityFilterDto>()), Times.Never);
    }

    [Fact]
    public async Task GetFilteredAsync_ShouldThrowKeyNotFoundException_WhenNoFlexibilitiesFound()
    {
        // Arrange
        var request = new PaginatedFlexibilityDtoRequest
        {
            Filter = new()
            {
                PageNumber = 1,
                PageSize = 2
            }
        };

        _mockCacheHandler.Setup(x => x.GetOrCreateRecordAsync(It.IsAny<PaginatedFlexibilityDtoRequest>(), It.IsAny<Func<Task<PaginatedFlexibilityDtoResponse>>>(), It.IsAny<CacheOptions>()))
            .Returns((PaginatedFlexibilityDtoRequest _, Func<Task<PaginatedFlexibilityDtoResponse>> factory, CacheOptions __) => factory());

        _mockRepository.Setup(repo => repo.GetFilteredAsync(It.IsAny<FlexibilityFilterDto>()))
            .ReturnsAsync(new List<FlexibilityDto>());

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetFilteredAsync(request));

        Assert.Equal(exception.Message, Messages.NotFound);
    }

    [Fact]
    public async Task GetFilteredAsync_ShouldReturnPaginatedData_WhenCacheMissAndDataExists()
    {
        // Arrange
        var request = new PaginatedFlexibilityDtoRequest
        {
            Filter = new()
            {
                PageNumber = 1,
                PageSize = 2
            }
        };
        var flexibilities = new List<FlexibilityDto>
        {
            new() { Id = _mockId },
            new() { Id = _mockId },
            new() { Id = _mockId }
        };

        _mockCacheHandler.Setup(x => x.GetOrCreateRecordAsync(It.IsAny<PaginatedFlexibilityDtoRequest>(), It.IsAny<Func<Task<PaginatedFlexibilityDtoResponse>>>(), It.IsAny<CacheOptions>()))
             .Returns((PaginatedFlexibilityDtoRequest _, Func<Task<PaginatedFlexibilityDtoResponse>> factory, CacheOptions __) => factory());

        _mockRepository.Setup(x => x.GetFilteredAsync(It.IsAny<FlexibilityFilterDto>()))
            .ReturnsAsync(flexibilities);

        // Act
        var result = await _service.GetFilteredAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Flexibilities.Count);
        Assert.Equal(3, result.TotalItems);
        Assert.Equal(2, result.TotalPages);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCachedData_WhenAvailable()
    {
        // Arrange
        var request = new GetFlexibilityDtoRequest
        {
            Id = _mockId
        };
        var expectedFlexibility = new FlexibilityDto
        {
            Id = _mockId
        };

        _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(expectedFlexibility);

        _mockCacheHandler.Setup(cache => cache.GetOrCreateRecordAsync(request, It.IsAny<Func<Task<GetFlexibilityDtoResponse>>>(), It.IsAny<CacheOptions>()))
            .ReturnsAsync(
                new GetFlexibilityDtoResponse
                {
                    Flexibility = expectedFlexibility
                });

        // Act
        var result = await _service.GetByIdAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(result.Flexibility.Id, request.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowKeyNotFoundException_WhenNoFlexibilityFound()
    {
        // Arrange
        var request = new GetFlexibilityDtoRequest
        {
            Id = _mockId
        };

        _mockCacheHandler.Setup(x => x.GetOrCreateRecordAsync(It.IsAny<GetFlexibilityDtoRequest>(), It.IsAny<Func<Task<GetFlexibilityDtoResponse>>>(), It.IsAny<CacheOptions>()))
            .Returns((GetFlexibilityDtoRequest _, Func<Task<GetFlexibilityDtoResponse>> factory, CacheOptions __) => factory());

        _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((FlexibilityDto)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetByIdAsync(request));

        Assert.Equal(exception.Message, Messages.NotFound);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnPaginatedData_WhenCacheMissAndDataExists()
    {
        // Arrange
        var request = new GetFlexibilityDtoRequest
        {
            Id = _mockId
        };
        var expectedFlexibility = new FlexibilityDto
        {
            Id = _mockId
        };

        _mockCacheHandler.Setup(x => x.GetOrCreateRecordAsync(It.IsAny<GetFlexibilityDtoRequest>(), It.IsAny<Func<Task<GetFlexibilityDtoResponse>>>(), It.IsAny<CacheOptions>()))
            .Returns((GetFlexibilityDtoRequest _, Func<Task<GetFlexibilityDtoResponse>> factory, CacheOptions __) => factory());

        _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
             .ReturnsAsync(expectedFlexibility);

        // Act
        var result = await _service.GetByIdAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(result.Flexibility.Id, request.Id);
    }
}
