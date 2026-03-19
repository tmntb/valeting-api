using Common.Enums;
using Common.Messages;
using FluentValidation;
using Moq;
using Service.Interfaces;
using Service.Models.Booking;
using Service.Models.Booking.Payload;
using Service.Models.Status;
using Service.Models.User;
using Service.Services;

namespace Service.Tests.Services;

public class BookingServiceTests
{
    private readonly BookingDto _bookingDto;
    private readonly StatusDto _statusDto;
    private readonly UserDto _userDto;

    private readonly Mock<IBookingRepository> _mockBookingRepository;
    private readonly Mock<IStatusRepository> _mockStatusRepository;
    private readonly BookingService _bookingService;

    public BookingServiceTests()
    {
        _bookingDto = DataFactory.CreateBookingDto();
        _statusDto = DataFactory.CreateStatusDto();
        _userDto = DataFactory.CreateUserDto();

        _mockBookingRepository = new Mock<IBookingRepository>();
        _mockStatusRepository = new Mock<IStatusRepository>();

        _bookingService = new BookingService(_mockBookingRepository.Object, _mockStatusRepository.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowValidationException_WhenRequestIsInvalid()
    {
        // Arrange
        var bookingDto = new BookingDto();

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _bookingService.CreateAsync(bookingDto));
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowKeyNotFoundException_WhenStatusDoesNotExist()
    {
        _mockStatusRepository
            .Setup(r => r.GetByCodeAsync(It.IsAny<StatusEnum>()))
            .ReturnsAsync((StatusDto)null)
            .Verifiable(Times.Once);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _bookingService.CreateAsync(_bookingDto));
        _mockStatusRepository.Verify();
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateBooking()
    {
        // Arrange
        _mockBookingRepository
            .Setup(r => r.CreateAsync(It.IsAny<BookingDto>()))
            .Returns(Task.CompletedTask)
            .Verifiable(Times.Once);

        _mockStatusRepository
            .Setup(r => r.GetByCodeAsync(It.IsAny<StatusEnum>()))
            .ReturnsAsync(_statusDto)
            .Verifiable(Times.Once);

        // Act
        var result = await _bookingService.CreateAsync(_bookingDto);

        // Assert
        Assert.NotEqual(Guid.Empty, result);

        _mockBookingRepository.Verify();
        _mockStatusRepository.Verify();
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowValidationException_WhenRequestIsInvalid()
    {
        // Arrange
        var bookingDto = new BookingDto();

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _bookingService.UpdateAsync(bookingDto));
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowKeyNotFoundException_WhenBookingNotFound()
    {
        // Arrange
        _mockBookingRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((BookingDto)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _bookingService.UpdateAsync(_bookingDto));

        Assert.Equal(Messages.NotFound, exception.Message);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowInvalidOperationException_WhenBookingStatusIsNotPendingApproval()
    {
        // Arrange
        _mockBookingRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(DataFactory.CreateBookingDto(statusEnum: StatusEnum.APPROVED))
            .Verifiable(Times.Once);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () => await _bookingService.UpdateAsync(_bookingDto));
        _mockBookingRepository.Verify();
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateBooking()
    {
        // Arrange
        _mockBookingRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_bookingDto)
            .Verifiable(Times.Once);

        _mockBookingRepository
            .Setup(r => r.UpdateAsync(It.IsAny<BookingDto>()))
            .Returns(Task.CompletedTask)
            .Verifiable(Times.Once);

        // Act
        await _bookingService.UpdateAsync(_bookingDto);

        // Assert
        _mockBookingRepository.Verify();
        _mockBookingRepository.Verify();
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldThrowKeyNotFoundException_WhenBookingNotFound()
    {
        // Arrange
        _mockBookingRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((BookingDto)null)
            .Verifiable(Times.Once);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _bookingService.UpdateStatusAsync(new()));
        _mockBookingRepository.Verify();
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldThrowKeyNotFoundException_WhenStatusNotFound()
    {
        // Arrange
        _mockBookingRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_bookingDto)
            .Verifiable(Times.Once);

        _mockStatusRepository
            .Setup(r => r.GetByCodeAsync(It.IsAny<StatusEnum>()))
            .ReturnsAsync((StatusDto)null)
            .Verifiable(Times.Once);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _bookingService.UpdateStatusAsync(new()));

        Assert.Equal(Messages.NotFound, exception.Message);
        _mockBookingRepository.Verify();
        _mockStatusRepository.Verify();
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldThrowValidationException_WhenRequestIsInvalid()
    {
        // Arrange
        _mockBookingRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_bookingDto)
            .Verifiable(Times.Once);

        _mockStatusRepository
            .Setup(r => r.GetByCodeAsync(It.IsAny<StatusEnum>()))
            .ReturnsAsync(_statusDto)
            .Verifiable(Times.Once);

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(async () => await _bookingService.UpdateStatusAsync(new()
        {
            UserDto = _userDto
        }));
        _mockBookingRepository.Verify();
        _mockStatusRepository.Verify();
    }

    [Theory]
    [InlineData(StatusEnum.APPROVED)]
    [InlineData(StatusEnum.REJECTED)]
    public async Task UpdateStatusAsync_ShouldUpdateStatus_And_UpdateDecison(StatusEnum statusEnum)
    {
        // Arrange
        _mockBookingRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_bookingDto)
            .Verifiable(Times.Once);

        _mockStatusRepository
            .Setup(r => r.GetByCodeAsync(It.IsAny<StatusEnum>()))
            .ReturnsAsync(_statusDto)
            .Verifiable(Times.Once);

        _mockBookingRepository
            .Setup(r => r.UpdateAsync(It.IsAny<BookingDto>()))
            .Returns(Task.CompletedTask)
            .Verifiable(Times.Once);

        // Act
        await _bookingService.UpdateStatusAsync(new()
        {
            Id = _bookingDto.Id,
            Status = statusEnum,
            UserDto = DataFactory.CreateUserDto(roleEnum: RoleEnum.ADMIN)
        });

        // Assert
        _mockBookingRepository.Verify();
        _mockStatusRepository.Verify();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowKeyNotFoundException_WhenNoBookingFound()
    {
        // Arrange
        _mockBookingRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((BookingDto)null)
            .Verifiable(Times.Once);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _bookingService.GetByIdAsync(_bookingDto.Id));
        _mockBookingRepository.Verify();
    }

    [Fact]
    public async Task GetByIdAsync__ShouldReturnPaginatedData_WhenBookingIsNotCompletedOrExpired()
    {
        // Arrange
        _mockBookingRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_bookingDto)
            .Verifiable(Times.Once);

        // Act
        var result = await _bookingService.GetByIdAsync(_bookingDto.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(_bookingDto.Id, result.Id);
        Assert.Equal(_bookingDto.Status.Code, result.Status.Code);
        Assert.Null(result.UpdatedAt);
        Assert.True(result.RequiresApproval);
        _mockBookingRepository.Verify();
    }

    [Fact]
    public async Task GetByIdAsync__ShouldReturnPaginatedData_WhenBookingIsCompleted()
    {
        // Arrange
        var bookingDto = DataFactory.CreateBookingDto(statusEnum: StatusEnum.APPROVED, scheduledAt: DateTime.UtcNow.AddDays(-2));

        _mockBookingRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(bookingDto)
            .Verifiable(Times.Once);

        _mockStatusRepository
            .Setup(repo => repo.GetByCodeAsync(It.IsAny<StatusEnum>()))
            .ReturnsAsync(DataFactory.CreateStatusDto(statusEnum: StatusEnum.COMPLETED))
            .Verifiable(Times.Once);

        _mockBookingRepository
            .Setup(repo => repo.UpdateAsync(It.IsAny<BookingDto>()))
            .Returns(Task.CompletedTask)
            .Verifiable(Times.Once);

        // Act
        var result = await _bookingService.GetByIdAsync(bookingDto.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(bookingDto.Id, result.Id);
        Assert.Equal(StatusEnum.COMPLETED, result.Status.Code);
        Assert.NotNull(result.UpdatedAt);
        Assert.False(result.RequiresApproval);
        _mockBookingRepository.Verify();
        _mockStatusRepository.Verify();
    }

    [Fact]
    public async Task GetByIdAsync__ShouldReturnPaginatedData_WhenBookingIsExpired()
    {
        // Arrange
        var bookingDto = DataFactory.CreateBookingDto(statusEnum: StatusEnum.PENDING_APPROVAL, scheduledAt: DateTime.UtcNow.AddDays(-1));

        _mockBookingRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(bookingDto)
            .Verifiable(Times.Once);

        _mockStatusRepository
            .Setup(repo => repo.GetByCodeAsync(It.IsAny<StatusEnum>()))
            .ReturnsAsync(DataFactory.CreateStatusDto(statusEnum: StatusEnum.EXPIRED))
            .Verifiable(Times.Once);

        _mockBookingRepository
            .Setup(repo => repo.UpdateAsync(It.IsAny<BookingDto>()))
            .Returns(Task.CompletedTask)
            .Verifiable(Times.Once);

        // Act
        var result = await _bookingService.GetByIdAsync(bookingDto.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(bookingDto.Id, result.Id);
        Assert.Equal(StatusEnum.EXPIRED, result.Status.Code);
        Assert.NotNull(result.UpdatedAt);
        Assert.False(result.RequiresApproval);
        _mockBookingRepository.Verify();
        _mockStatusRepository.Verify();
    }

    [Fact]
    public async Task GetCustomerFilteredAsync_ShouldThrowKeyNotFoundException_WhenNoBookingFound()
    {
        // Arrange
        _mockBookingRepository
            .Setup(repo => repo.GetFilteredAsync(It.IsAny<BookingFilterDto>()))
            .ReturnsAsync([])
            .Verifiable(Times.Once);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _bookingService.GetCustomerFilteredAsync(new()
        {
            PageNumber = 1,
            PageSize = 10
        }));

        _mockBookingRepository.Verify();
    }

    [Fact]
    public async Task GetCustomerFilteredAsync_ShouldReturnPaginatedData_WhenBookingFound()
    {
        // Arrange
        _mockBookingRepository
            .Setup(repo => repo.GetFilteredAsync(It.IsAny<BookingFilterDto>()))
            .ReturnsAsync([_bookingDto])
            .Verifiable(Times.Once);

        // Act
        var result = await _bookingService.GetCustomerFilteredAsync(
            new()
            {
                PageNumber = 1,
                PageSize = 10,
                CustomerId = _userDto.Id
            });

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Bookings);
        Assert.Equal(_bookingDto.Id, result.Bookings.First().Id);
        _mockBookingRepository.Verify();
    }

    [Fact]
    public async Task GetFilteredAsync_ShouldThrowKeyNotFoundException_WhenNoBookingFound()
    {
        // Arrange
        _mockBookingRepository
            .Setup(repo => repo.GetFilteredAsync(It.IsAny<BookingFilterDto>()))
            .ReturnsAsync([])
            .Verifiable(Times.Once);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _bookingService.GetFilteredAsync(new()
        {
            PageNumber = 1,
            PageSize = 10
        }));

        _mockBookingRepository.Verify();
    }

    [Fact]
    public async Task GetFilteredAsync_ShouldReturnPaginatedData_WhenBookingFound()
    {
        // Arrange
        _mockBookingRepository
            .Setup(repo => repo.GetFilteredAsync(It.IsAny<BookingFilterDto>()))
            .ReturnsAsync([_bookingDto])
            .Verifiable(Times.Once);

        // Act
        var result = await _bookingService.GetFilteredAsync(
            new()
            {
                PageNumber = 1,
                PageSize = 10,
                Status = StatusEnum.PENDING_APPROVAL
            });

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Bookings);
        Assert.Equal(_bookingDto.Id, result.Bookings.First().Id);
        _mockBookingRepository.Verify();
    }
}
