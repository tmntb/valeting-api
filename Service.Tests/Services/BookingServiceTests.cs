using Common.Enums;
using Common.Messages;
using Moq;
using Service.Interfaces;
using Service.Models.Booking;
using Service.Models.Booking.Payload;
using Service.Models.Status;
using Service.Services;

namespace Service.Tests.Services;

public class BookingServiceTests
{
    private readonly Mock<IBookingRepository> _mockBookingRepository;
    private readonly Mock<IStatusRepository> _mockStatusRepository;
    private readonly Guid _mockId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private readonly BookingService _bookingService;

    public BookingServiceTests()
    {
        _mockBookingRepository = new Mock<IBookingRepository>();
        _mockStatusRepository = new Mock<IStatusRepository>();

        _bookingService = new BookingService(_mockBookingRepository.Object, _mockStatusRepository.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateBooking()
    {
        // Arrange
        _mockBookingRepository
            .Setup(r => r.CreateAsync(It.IsAny<BookingDto>()))
            .Returns(Task.CompletedTask);

        _mockStatusRepository
            .Setup(r => r.GetByCodeAsync(It.IsAny<StatusEnum>()))
            .ReturnsAsync(new StatusDto());

        // Act
        var result = await _bookingService.CreateAsync(
            new()
            {
                Reference = "name",
                ScheduledAt = DateTime.Now.AddDays(1),
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
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowKeyNotFoundException_WhenBookingNotFound()
    {
        // Arrange
        _mockBookingRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((BookingDto)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _bookingService.UpdateAsync(
             new()
             {
                 Id = _mockId,
                 Reference = "name",
                 ScheduledAt = DateTime.Now.AddDays(1),
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
    public async Task UpdateAsync_ShouldUpdateBooking()
    {
        // Arrange
        _mockBookingRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new BookingDto()
            {
                Status = new()
                {
                    Code = StatusEnum.PENDING_APPROVAL
                }
            });

        _mockBookingRepository
            .Setup(r => r.UpdateAsync(It.IsAny<BookingDto>()))
            .Returns(Task.CompletedTask);

        // Act
        await _bookingService.UpdateAsync(
            new()
            {
                Id = _mockId,
                Reference = "name",
                ScheduledAt = DateTime.Now.AddDays(1),
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
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldThrowKeyNotFoundException_WhenBookingNotFound()
    {
        // Arrange
        _mockBookingRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((BookingDto)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _bookingService.UpdateStatusAsync(new()
        {
            Id = _mockId,
            Status = StatusEnum.APPROVED,
            UserDto = new()
            {
                Id = _mockId,
                Role = new()                
                {
                    Code = RoleEnum.ADMIN
                }
            }            
        }));

        Assert.Equal(Messages.NotFound, exception.Message);
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldThrowKeyNotFoundException_WhenStatusNotFound()
    {
        // Arrange
        _mockBookingRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new BookingDto()
            {
                Status = new()
                {
                    Code = StatusEnum.PENDING_APPROVAL
                }
            });

        _mockStatusRepository
            .Setup(r => r.GetByCodeAsync(It.IsAny<StatusEnum>()))
            .ReturnsAsync((StatusDto)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _bookingService.UpdateStatusAsync(new()
        {
            Id = _mockId,
            Status = StatusEnum.APPROVED,
            UserDto = new()
            {
                Id = _mockId,
                Role = new()                
                {
                    Code = RoleEnum.ADMIN
                }
            }            
        }));

        Assert.Equal(Messages.NotFound, exception.Message);
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldUpdateStatus()
    {
        // Arrange
        _mockBookingRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new BookingDto()
            {
                Status = new()
                {
                    Code = StatusEnum.PENDING_APPROVAL
                }
            });

        _mockStatusRepository
            .Setup(r => r.GetByCodeAsync(It.IsAny<StatusEnum>()))
            .ReturnsAsync(new StatusDto()
            {
                Code = StatusEnum.APPROVED
            });

        _mockBookingRepository
            .Setup(r => r.UpdateAsync(It.IsAny<BookingDto>()))
            .Returns(Task.CompletedTask);

        // Act
        await _bookingService.UpdateStatusAsync(new()
        {
            Id = _mockId,
            Status = StatusEnum.APPROVED,
            UserDto = new()
            {
                Id = _mockId,
                Role = new()                
                {
                    Code = RoleEnum.ADMIN
                }
            }            
        });

        // Assert
        _mockBookingRepository.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
        _mockBookingRepository.Verify(r => r.UpdateAsync(It.IsAny<BookingDto>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowKeyNotFoundException_WhenNoBookingFound()
    {
        // Arrange
        _mockBookingRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((BookingDto)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _bookingService.GetByIdAsync(_mockId));

        Assert.Equal(exception.Message, Messages.NotFound);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnPaginatedData()
    {
        // Arrange
        _mockBookingRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
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
    public async Task GetFilteredAsync_ShouldThrowKeyNotFoundException_WhenNoBookingFound()
    {
        // Arrange
        _mockBookingRepository
            .Setup(repo => repo.GetFilteredAsync(It.IsAny<BookingFilterDto>()))
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
}
