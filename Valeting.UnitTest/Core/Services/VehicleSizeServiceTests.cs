using Moq;
using Valeting.Common.Cache;
using Valeting.Common.Cache.Interfaces;
using Valeting.Common.Messages;
using Valeting.Common.Models.VehicleSize;
using Valeting.Core.Interfaces;
using Valeting.Core.Services;
using Valeting.Repository.Interfaces;

namespace Valeting.Tests.Core.Services;

public class VehicleSizeServiceTests
{
    private readonly Mock<IVehicleSizeRepository> _mockRepository;
    private readonly Mock<ICacheHandler> _mockCacheHandler;

    private readonly Guid _mockId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private readonly VehicleSizeService _service;

    public VehicleSizeServiceTests()
    {
        _mockRepository = new Mock<IVehicleSizeRepository>();
        _mockCacheHandler = new Mock<ICacheHandler>();

        _service = new VehicleSizeService(_mockRepository.Object, _mockCacheHandler.Object);
    }

    [Fact]
    public async Task GetFilteredAsync_ShouldReturnCachedData_WhenAvailable()
    {
        // Arrange
        var request = new PaginatedVehicleSizeDtoRequest
        {
            Filter = new() 
            { 
                PageNumber = 1, 
                PageSize = 2 
            }
        };
        var vehicleSizes = new List<VehicleSizeDto>()
        {
            new() { Id = _mockId },
            new() { Id = _mockId }
        };

        _mockCacheHandler.Setup(cache => cache.GetOrCreateRecordAsync(request, It.IsAny<Func<Task<PaginatedVehicleSizeDtoResponse>>>(), It.IsAny<CacheOptions>()))
            .ReturnsAsync(
                new PaginatedVehicleSizeDtoResponse
                {
                    VehicleSizes = vehicleSizes,
                    TotalItems = 2,
                    TotalPages = 1
                });

        // Act
        var result = await _service.GetFilteredAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalItems);
        Assert.Equal(1, result.TotalPages);
        _mockRepository.Verify(x => x.GetFilteredAsync(It.IsAny<VehicleSizeFilterDto>()), Times.Never);
    }

    [Fact]
    public async Task GetFilteredAsync_ShouldThrowKeyNotFoundException_WhenNoVehicleSizesFound()
    {
        // Arrange
        var request = new PaginatedVehicleSizeDtoRequest 
        {
            Filter = new() 
            { 
                PageNumber = 1, 
                PageSize = 2 
            } 
        };

        _mockCacheHandler.Setup(x => x.GetOrCreateRecordAsync(It.IsAny<PaginatedVehicleSizeDtoRequest>(), It.IsAny<Func<Task<PaginatedVehicleSizeDtoResponse>>>(), It.IsAny<CacheOptions>()))
            .Returns((PaginatedVehicleSizeDtoRequest _, Func<Task<PaginatedVehicleSizeDtoResponse>> factory, CacheOptions __) => factory());

        _mockRepository.Setup(repo => repo.GetFilteredAsync(It.IsAny<VehicleSizeFilterDto>()))
            .ReturnsAsync(new List<VehicleSizeDto>());

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetFilteredAsync(request));

        Assert.Equal(exception.Message, Messages.NotFound);
    }

    [Fact]
    public async Task GetFilteredAsync_ShouldReturnPaginatedData_WhenCacheMissAndDataExists()
    {
        // Arrange
        var request = new PaginatedVehicleSizeDtoRequest 
        { 
            Filter = new() 
            { 
                PageNumber = 1, 
                PageSize = 2
            } 
        };
        var vehicleSizes = new List<VehicleSizeDto>
        {
            new() { Id = _mockId },
            new() { Id = _mockId },
            new() { Id = _mockId }
        };

        _mockCacheHandler.Setup(x => x.GetOrCreateRecordAsync(It.IsAny<PaginatedVehicleSizeDtoRequest>(), It.IsAny<Func<Task<PaginatedVehicleSizeDtoResponse>>>(), It.IsAny<CacheOptions>()))
             .Returns((PaginatedVehicleSizeDtoRequest _, Func<Task<PaginatedVehicleSizeDtoResponse>> factory, CacheOptions __) => factory());

        _mockRepository.Setup(x => x.GetFilteredAsync(request.Filter))
            .ReturnsAsync(vehicleSizes);

        // Act
        var result = await _service.GetFilteredAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.VehicleSizes.Count);
        Assert.Equal(3, result.TotalItems);
        Assert.Equal(2, result.TotalPages);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCachedData_WhenAvailable()
    {
        // Arrange
        var request = new GetVehicleSizeDtoRequest
        {
            Id = _mockId
        };
        var expectedVehicleSize = new VehicleSizeDto
        {
            Id = _mockId
        };

        _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(expectedVehicleSize);

        _mockCacheHandler.Setup(cache => cache.GetOrCreateRecordAsync(request, It.IsAny<Func<Task<GetVehicleSizeDtoResponse>>>(), It.IsAny<CacheOptions>()))
            .ReturnsAsync(
                new GetVehicleSizeDtoResponse
                {
                    VehicleSize = expectedVehicleSize
                });

        // Act
        var result = await _service.GetByIdAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(result.VehicleSize.Id, request.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowKeyNotFoundException_WhenNoVehicleSizeFound()
    {
        // Arrange
        var request = new GetVehicleSizeDtoRequest
        {
            Id = _mockId
        };

        _mockCacheHandler.Setup(x => x.GetOrCreateRecordAsync(It.IsAny<GetVehicleSizeDtoRequest>(), It.IsAny<Func<Task<GetVehicleSizeDtoResponse>>>(), It.IsAny<CacheOptions>()))
            .Returns((GetVehicleSizeDtoRequest _, Func<Task<GetVehicleSizeDtoResponse>> factory, CacheOptions __) => factory());

        _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((VehicleSizeDto)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetByIdAsync(request));

        Assert.Equal(exception.Message, Messages.NotFound);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnPaginatedData_WhenCacheMissAndDataExists()
    {
        // Arrange
        var request = new GetVehicleSizeDtoRequest
        {
            Id = _mockId
        };
        var expectedVehicleSize = new VehicleSizeDto
        {
            Id = _mockId
        };

        _mockCacheHandler.Setup(x => x.GetOrCreateRecordAsync(It.IsAny<GetVehicleSizeDtoRequest>(), It.IsAny<Func<Task<GetVehicleSizeDtoResponse>>>(), It.IsAny<CacheOptions>()))
            .Returns((GetVehicleSizeDtoRequest _, Func<Task<GetVehicleSizeDtoResponse>> factory, CacheOptions __) => factory());

        _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
             .ReturnsAsync(expectedVehicleSize);

        // Act
        var result = await _service.GetByIdAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(result.VehicleSize.Id, request.Id);
    }
}
