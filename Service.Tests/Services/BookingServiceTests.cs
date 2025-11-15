using Common.Cache;
using Common.Cache.Interfaces;
using Common.Messages;
using Moq;
using Service.Interfaces;
using Service.Models.Booking;
using Service.Models.Booking.Payload;
using Service.Services;

namespace Service.Tests.Services;

public class BookingServiceTests
{
    private readonly Mock<IBookingRepository> _mockBookingRepository;
    private readonly Mock<ICacheHandler> _mockCacheHandler;

    private readonly Guid _mockId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private readonly BookingService _bookingService;

    public BookingServiceTests()
    {
        _mockBookingRepository = new Mock<IBookingRepository>();
        _mockCacheHandler = new Mock<ICacheHandler>();

        _bookingService = new BookingService(_mockBookingRepository.Object, _mockCacheHandler.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateBookingAndInvalidateCache()
    {
        // Arrange
        _mockBookingRepository.Setup(r => r.CreateAsync(It.IsAny<BookingDto>()))
            .Returns(Task.CompletedTask);

        _mockCacheHandler.Setup(c => c.InvalidateCacheByListType(It.IsAny<CacheListType>()));

        // Act
        var result = await _bookingService.CreateAsync(
            new()
            {
                Name = "name",
                BookingDate = DateTime.Now.AddDays(1),
                ContactNumber = 123,
                Email = "email",
                Flexibility = new()
                {
                    Id = _mockId
                },
                VehicleSize = new()
                {
                    Id = _mockId
                }
            });

        // Assert
        Assert.NotEqual(Guid.Empty, result);

        _mockBookingRepository.Verify(r => r.CreateAsync(It.IsAny<BookingDto>()), Times.Once);
        _mockCacheHandler.Verify(c => c.InvalidateCacheByListType(CacheListType.Booking), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowKeyNotFoundException_WhenBookingNotFound()
    {
        // Arrange
        _mockBookingRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((BookingDto)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _bookingService.UpdateAsync(
             new()
             {
                 Id = _mockId,
                 Name = "name",
                 BookingDate = DateTime.Now.AddDays(1),
                 ContactNumber = 123,
                 Email = "email",
                 Flexibility = new()
                 {
                     Id = _mockId
                 },
                 VehicleSize = new()
                 {
                     Id = _mockId
                 }
             }));

        Assert.Equal(Messages.NotFound, exception.Message);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateBookingAndInvalidateCache()
    {
        // Arrange
        _mockBookingRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new BookingDto());

        _mockBookingRepository.Setup(r => r.UpdateAsync(It.IsAny<BookingDto>()))
            .Returns(Task.CompletedTask);

        _mockCacheHandler.Setup(c => c.InvalidateCacheById(It.IsAny<Guid>()));
        _mockCacheHandler.Setup(c => c.InvalidateCacheByListType(It.IsAny<CacheListType>()));

        // Act
        await _bookingService.UpdateAsync(
            new()
            {
                Id = _mockId,
                Name = "name",
                BookingDate = DateTime.Now.AddDays(1),
                ContactNumber = 123,
                Email = "email",
                Flexibility = new()
                {
                    Id = _mockId
                },
                VehicleSize = new()
                {
                    Id = _mockId
                }
            });

        // Assert
        _mockBookingRepository.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
        _mockBookingRepository.Verify(r => r.UpdateAsync(It.IsAny<BookingDto>()), Times.Once);
        _mockCacheHandler.Verify(c => c.InvalidateCacheById(It.IsAny<Guid>()), Times.Once);
        _mockCacheHandler.Verify(c => c.InvalidateCacheByListType(CacheListType.Booking), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowKeyNotFoundException_WhenBookingNotFound()
    {
        // Arrange
        _mockBookingRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((BookingDto)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _bookingService.DeleteAsync(_mockId));

        Assert.Equal(Messages.NotFound, exception.Message);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteBookingAndInvalidateCache()
    {
        // Arrange
        _mockBookingRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new BookingDto());

        _mockBookingRepository.Setup(r => r.DeleteAsync(It.IsAny<Guid>()))
            .Returns(Task.CompletedTask);

        _mockCacheHandler.Setup(c => c.InvalidateCacheById(It.IsAny<Guid>()));
        _mockCacheHandler.Setup(c => c.InvalidateCacheByListType(It.IsAny<CacheListType>()));

        // Act
        await _bookingService.DeleteAsync(_mockId);

        // Assert
        _mockBookingRepository.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
        _mockBookingRepository.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Once);
        _mockCacheHandler.Verify(c => c.InvalidateCacheById(It.IsAny<Guid>()), Times.Once);
        _mockCacheHandler.Verify(c => c.InvalidateCacheByListType(CacheListType.Booking), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCachedData()
    {
        // Arrange
        _mockCacheHandler.Setup(c => c.GetOrCreateRecordAsync(It.IsAny<Guid>(), It.IsAny<Func<Task<BookingDto>>>(), It.IsAny<CacheOptions>()))
            .ReturnsAsync(
                new BookingDto
                {
                    Id = _mockId
                });

        // Act
        var result = await _bookingService.GetByIdAsync(_mockId);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result);
        Assert.Equal(_mockId, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowKeyNotFoundException_WhenNoBookingFound()
    {
        // Arrange
        _mockCacheHandler.Setup(c => c.GetOrCreateRecordAsync(It.IsAny<Guid>(), It.IsAny<Func<Task<BookingDto>>>(), It.IsAny<CacheOptions>()))
            .Returns((Guid _, Func<Task<BookingDto>> factory, CacheOptions __) => factory());

        _mockBookingRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((BookingDto)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _bookingService.GetByIdAsync(_mockId));

        Assert.Equal(exception.Message, Messages.NotFound);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnPaginatedData_WhenCacheMissAndDataExists()
    {
        // Arrange
        _mockCacheHandler.Setup(c => c.GetOrCreateRecordAsync(It.IsAny<Guid>(), It.IsAny<Func<Task<BookingDto>>>(), It.IsAny<CacheOptions>()))
             .Returns((Guid _, Func<Task<BookingDto>> factory, CacheOptions __) => factory());

        _mockBookingRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(
                new BookingDto
                {
                    Id = _mockId
                });

        // Act
        var result = await _bookingService.GetByIdAsync(_mockId);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result);
        Assert.Equal(_mockId, result.Id);
    }

    [Fact]
    public async Task GetFilteredAsync_Should_ReturnPaginatedBookings()
    {
        // Arrange
        _mockCacheHandler.Setup(c => c.GetOrCreateRecordAsync(It.IsAny<BookingFilterDto>(), It.IsAny<Func<Task<BookingPaginatedDtoResponse>>>(), It.IsAny<CacheOptions>()))
            .ReturnsAsync(
                new BookingPaginatedDtoResponse
                {
                    Bookings =
                    [
                        new(),
                        new()
                    ],
                    TotalItems = 2,
                    TotalPages = 1
                });

        // Act
        var result = await _bookingService.GetFilteredAsync(
            new()
            {
                PageNumber = 1,
                PageSize = 10
            });

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalItems);
        Assert.Equal(1, result.TotalPages);
        _mockBookingRepository.Verify(x => x.GetFilteredAsync(It.IsAny<BookingFilterDto>()), Times.Never);
    }

    [Fact]
    public async Task GetFilteredAsync_ShouldThrowKeyNotFoundException_WhenNoBookingFound()
    {
        // Arrange
        _mockCacheHandler.Setup(x => x.GetOrCreateRecordAsync(It.IsAny<BookingFilterDto>(), It.IsAny<Func<Task<BookingPaginatedDtoResponse>>>(), It.IsAny<CacheOptions>()))
            .Returns((BookingFilterDto _, Func<Task<BookingPaginatedDtoResponse>> factory, CacheOptions __) => factory());

        _mockBookingRepository.Setup(repo => repo.GetFilteredAsync(It.IsAny<BookingFilterDto>()))
            .ReturnsAsync(new List<BookingDto>());

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _bookingService.GetFilteredAsync(
            new()
            {
                PageNumber = 1,
                PageSize = 10
            }));

        Assert.Equal(exception.Message, Messages.NotFound);
    }

    [Fact]
    public async Task GetFilteredAsync_ShouldReturnPaginatedData_WhenCacheMissAndDataExists()
    {
        // Arrange
        _mockCacheHandler.Setup(x => x.GetOrCreateRecordAsync(It.IsAny<BookingFilterDto>(), It.IsAny<Func<Task<BookingPaginatedDtoResponse>>>(), It.IsAny<CacheOptions>()))
            .Returns((BookingFilterDto _, Func<Task<BookingPaginatedDtoResponse>> factory, CacheOptions __) => factory());

        _mockBookingRepository.Setup(repo => repo.GetFilteredAsync(It.IsAny<BookingFilterDto>()))
           .ReturnsAsync(
                [
                    new(),
                    new(),
                    new()
                ]);

        // Act
        var result = await _bookingService.GetFilteredAsync(
            new()
            {
                PageNumber = 1,
                PageSize = 2
            });

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Bookings.Count);
        Assert.Equal(3, result.TotalItems);
        Assert.Equal(2, result.TotalPages);
    }
}
