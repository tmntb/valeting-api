using Moq;
using Common.Cache;
using Common.Cache.Interfaces;
using Common.Messages;
using Common.Models.VehicleSize;
using Service.Interfaces;
using Service.Services;

namespace Service.Tests.Services;

public class VehicleSizeServiceTests
{
    private readonly Mock<IVehicleSizeRepository> _mockVehicleSizeRepository;
    private readonly Mock<ICacheHandler> _mockCacheHandler;

    private readonly Guid _mockId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private readonly VehicleSizeService _vehicleSizeService;

    public VehicleSizeServiceTests()
    {
        _mockVehicleSizeRepository = new Mock<IVehicleSizeRepository>();
        _mockCacheHandler = new Mock<ICacheHandler>();

        _vehicleSizeService = new VehicleSizeService(_mockVehicleSizeRepository.Object, _mockCacheHandler.Object);
    }

    [Fact]
    public async Task GetFilteredAsync_ShouldReturnCachedData_WhenAvailable()
    {
        // Arrange
        _mockCacheHandler.Setup(cache => cache.GetOrCreateRecordAsync(It.IsAny<PaginatedVehicleSizeDtoRequest>(), It.IsAny<Func<Task<PaginatedVehicleSizeDtoResponse>>>(), It.IsAny<CacheOptions>()))
            .ReturnsAsync(
                new PaginatedVehicleSizeDtoResponse
                {
                    VehicleSizes =
                    [
                        new() { Id = _mockId },
                        new() { Id = _mockId }
                    ],
                    TotalItems = 2,
                    TotalPages = 1
                });

        // Act
        var result = await _vehicleSizeService.GetFilteredAsync(
            new()
            {
                Filter = new()
                {
                    PageNumber = 1,
                    PageSize = 2
                }
            });

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalItems);
        Assert.Equal(1, result.TotalPages);
        _mockVehicleSizeRepository.Verify(x => x.GetFilteredAsync(It.IsAny<VehicleSizeFilterDto>()), Times.Never);
    }

    [Fact]
    public async Task GetFilteredAsync_ShouldThrowKeyNotFoundException_WhenNoVehicleSizesFound()
    {
        // Arrange
        _mockCacheHandler.Setup(x => x.GetOrCreateRecordAsync(It.IsAny<PaginatedVehicleSizeDtoRequest>(), It.IsAny<Func<Task<PaginatedVehicleSizeDtoResponse>>>(), It.IsAny<CacheOptions>()))
            .Returns((PaginatedVehicleSizeDtoRequest _, Func<Task<PaginatedVehicleSizeDtoResponse>> factory, CacheOptions __) => factory());

        _mockVehicleSizeRepository.Setup(repo => repo.GetFilteredAsync(It.IsAny<VehicleSizeFilterDto>()))
            .ReturnsAsync(new List<VehicleSizeDto>());

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _vehicleSizeService.GetFilteredAsync(
            new()
            {
                Filter = new()
                {
                    PageNumber = 1,
                    PageSize = 2
                }
            }));

        Assert.Equal(exception.Message, Messages.NotFound);
    }

    [Fact]
    public async Task GetFilteredAsync_ShouldReturnPaginatedData_WhenCacheMissAndDataExists()
    {
        // Arrange
        _mockCacheHandler.Setup(x => x.GetOrCreateRecordAsync(It.IsAny<PaginatedVehicleSizeDtoRequest>(), It.IsAny<Func<Task<PaginatedVehicleSizeDtoResponse>>>(), It.IsAny<CacheOptions>()))
             .Returns((PaginatedVehicleSizeDtoRequest _, Func<Task<PaginatedVehicleSizeDtoResponse>> factory, CacheOptions __) => factory());

        _mockVehicleSizeRepository.Setup(x => x.GetFilteredAsync(It.IsAny<VehicleSizeFilterDto>()))
            .ReturnsAsync(
                [
                    new() { Id = _mockId },
                    new() { Id = _mockId },
                    new() { Id = _mockId }
                ]);

        // Act
        var result = await _vehicleSizeService.GetFilteredAsync(
            new()
            {
                Filter = new()
                {
                    PageNumber = 1,
                    PageSize = 2
                }
            });

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
        _mockVehicleSizeRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new VehicleSizeDto
            {
                Id = _mockId
            });

        _mockCacheHandler.Setup(cache => cache.GetOrCreateRecordAsync(It.IsAny<GetVehicleSizeDtoRequest>(), It.IsAny<Func<Task<GetVehicleSizeDtoResponse>>>(), It.IsAny<CacheOptions>()))
            .ReturnsAsync(
                new GetVehicleSizeDtoResponse
                {
                    VehicleSize = new()
                    {
                        Id = _mockId
                    }
                });

        // Act
        var result = await _vehicleSizeService.GetByIdAsync(
            new()
            {
                Id = _mockId
            });

        // Assert
        Assert.NotNull(result);
        Assert.Equal(_mockId, result.VehicleSize.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowKeyNotFoundException_WhenNoVehicleSizeFound()
    {
        // Arrange
        _mockCacheHandler.Setup(x => x.GetOrCreateRecordAsync(It.IsAny<GetVehicleSizeDtoRequest>(), It.IsAny<Func<Task<GetVehicleSizeDtoResponse>>>(), It.IsAny<CacheOptions>()))
            .Returns((GetVehicleSizeDtoRequest _, Func<Task<GetVehicleSizeDtoResponse>> factory, CacheOptions __) => factory());

        _mockVehicleSizeRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((VehicleSizeDto)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _vehicleSizeService.GetByIdAsync(
            new()
            {
                Id = _mockId
            }));

        Assert.Equal(exception.Message, Messages.NotFound);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnPaginatedData_WhenCacheMissAndDataExists()
    {
        // Arrange
        var expectedVehicleSize = new VehicleSizeDto
        {
            Id = _mockId
        };

        _mockCacheHandler.Setup(x => x.GetOrCreateRecordAsync(It.IsAny<GetVehicleSizeDtoRequest>(), It.IsAny<Func<Task<GetVehicleSizeDtoResponse>>>(), It.IsAny<CacheOptions>()))
            .Returns((GetVehicleSizeDtoRequest _, Func<Task<GetVehicleSizeDtoResponse>> factory, CacheOptions __) => factory());

        _mockVehicleSizeRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
             .ReturnsAsync(
                new VehicleSizeDto
                {
                    Id = _mockId
                });

        // Act
        var result = await _vehicleSizeService.GetByIdAsync(
            new()
            {
                Id = _mockId
            });

        // Assert
        Assert.NotNull(result);
        Assert.Equal(_mockId, result.VehicleSize.Id);
    }
}
