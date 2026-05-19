using Api.Controllers;
using Api.Models.Booking.Payload;
using Common.Enums;
using Common.Messages;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Service.Interfaces;
using Service.Models.Booking;
using Service.Models.Booking.Payload;
using Service.Models.Link.Payload;
using System.Net;

namespace Api.Tests.Controllers;

public class BookingControllerTests
{
    private readonly ClaimsFixture _claimsFixture = new();
    private readonly Mock<IBookingService> _mockBookingService;
    private readonly Mock<ILinkService> _mockLinkService;

    private readonly Guid _mockBookingId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private readonly Guid _mockFlexibilityId = Guid.Parse("00000000-0000-0000-0000-000000000002");
    private readonly Guid _mockVehicleSizeId = Guid.Parse("00000000-0000-0000-0000-000000000003");
    private readonly BookingController _bookingController;

    public BookingControllerTests()
    {
        _mockBookingService = new Mock<IBookingService>();
        _mockLinkService = new Mock<ILinkService>();

        _bookingController = new BookingController(_mockBookingService.Object, _mockLinkService.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowArgumentNullException_WhenParamsAreNull()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _bookingController.CreateAsync(null));
        Assert.Contains(Messages.InvalidRequestBody, exception.Message);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnCreated_WhenValidRequest()
    {
        // Arrange
        _claimsFixture.SetupUserClaims(_bookingController, Guid.Parse("00000000-0000-0000-0000-000000000099"));

        _mockBookingService
            .Setup(s => s.CreateAsync(It.IsAny<BookingDto>()))
            .ReturnsAsync(_mockBookingId);

        _mockLinkService
            .Setup(u => u.GenerateSelf(It.IsAny<GenerateSelfLinkDtoRequest>()))
            .Returns($"https://api.test.com/bookings/{_mockBookingId}");

        // Act
        var result = await _bookingController.CreateAsync(
            new()
            {
                ScheduledAt = DateTime.Now.AddDays(1),
                FlexibilityId = _mockFlexibilityId,
                VehicleSizeId = _mockVehicleSizeId,
                Notes = "Test notes"
            }
        ) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.Created, result.StatusCode);

        var responseApi = (CreateBookingApiResponse)result.Value;
        Assert.Equal(_mockBookingId, responseApi.Id);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowArgumentNullException_WhenIdIsNull()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _bookingController.UpdateAsync(null, null));
        Assert.Contains(Messages.InvalidRequestId, exception.Message);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowArgumentNullException_WhenParamsAreNull()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _bookingController.UpdateAsync(_mockBookingId.ToString(), null));
        Assert.Contains(Messages.InvalidRequestBody, exception.Message);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNoContent_WhenValidRequest()
    {
        // Arrange
        _mockBookingService
            .Setup(s => s.UpdateAsync(It.IsAny<BookingDto>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _bookingController.UpdateAsync
        (
            _mockBookingId.ToString(),
            new()
            {
                ScheduledAt = DateTime.Now.AddDays(1),
                FlexibilityId = _mockFlexibilityId,
                VehicleSizeId = _mockVehicleSizeId,
                Notes = "Updated test notes"
            }
        ) as StatusCodeResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.NoContent, result.StatusCode);
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldThrowArgumentNullException_WhenIdIsNull()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _bookingController.UpdateStatusAsync(null, null));
        Assert.Contains(Messages.InvalidRequestId, exception.Message);
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldReturnNoContent_WhenValidId()
    {
        // Arrange
        _claimsFixture.SetupUserClaims(_bookingController, Guid.Parse("00000000-0000-0000-0000-000000000099"));

        _mockBookingService
            .Setup(s => s.UpdateStatusAsync(It.IsAny<UpdateBookingStatusDtoRequest>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _bookingController.UpdateStatusAsync(
            _mockBookingId.ToString(),
            new()
            {
                Status = StatusEnum.APPROVED,
            }
        ) as StatusCodeResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.NoContent, result.StatusCode);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowArgumentNullException_WhenIdIsNull()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _bookingController.GetByIdAsync(null));
        Assert.Contains(Messages.InvalidRequestId, exception.Message);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnBooking_WhenValidId()
    {
        // Arrange
        _claimsFixture.SetupUserClaims(_bookingController, Guid.Parse("00000000-0000-0000-0000-000000000099"));

        _mockBookingService
            .Setup(s => s.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(
                new BookingDto()
                {
                    Id = _mockBookingId,
                    Reference = "REF123",
                    Customer = new()
                    {
                        ContactNumber = 1234567890,
                        Email = "test@example.com",
                        Role = new()
                        {
                            Name = RoleEnum.USER.ToString()
                        }
                    },
                    Flexibility = new()
                    {
                        Name = FlexibilityEnum.FLEX_2D.ToString()
                    },
                    VehicleSize = new()
                    {
                        Name = VehicleSizeEnum.MOTORCYCLE.ToString()
                    },
                    ScheduledAt = DateTime.Now.AddDays(1),
                    Status = new()
                    {
                        Name = StatusEnum.APPROVED.ToString()
                    },
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now.AddHours(1),
                    DecisionAt = DateTime.Now.AddHours(1),
                    Decision = new()
                    {
                        Email = "admin@example.com",
                        Role = new()
                        {
                            Name = RoleEnum.ADMIN.ToString()
                        }
                    },
                    RequiresApproval = false,
                    Notes = "Test notes"
                });

        _mockLinkService
            .Setup(u => u.GenerateSelf(It.IsAny<GenerateSelfLinkDtoRequest>()))
            .Returns($"https://api.test.com/bookings/{_mockBookingId}");

        // Act
        var result = await _bookingController.GetByIdAsync(_mockBookingId.ToString()) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);

        var responseApi = (BookingApiResponse)result.Value;
        Assert.Equal(_mockBookingId, responseApi.Booking.Id);
        Assert.Equal($"https://api.test.com/bookings/{_mockBookingId}", responseApi.Booking.Link.Self.Href);
    }

    [Fact]
    public async Task GetCustomerFilteredAsync_ShouldThrowArgumentNullException_WhenParamsAreNull()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _bookingController.GetCustomerFilteredAsync(null));
        Assert.Contains(Messages.InvalidRequestQueryParameters, exception.Message);
    }

    [Fact]
    public async Task GetCustomerFilteredAsync_ShouldReturnPagedResponse_WhenValidRequest()
    {
        // Arrange
        _claimsFixture.SetupUserClaims(_bookingController, Guid.Parse("00000000-0000-0000-0000-000000000099"));

        _mockBookingService
            .Setup(s => s.GetCustomerFilteredAsync(It.IsAny<BookingFilterDto>()))
            .ReturnsAsync(
                new BookingPaginatedDtoResponse
                {
                    TotalItems = 1,
                    TotalPages = 1,
                    Bookings =
                    [
                        new()
                        {
                            Id = _mockBookingId,
                            Reference = "REF123",
                            Customer = new()
                            {
                                ContactNumber = 1234567890,
                                Email = "test@example.com",
                                Role = new()
                                {
                                    Name = RoleEnum.USER.ToString()
                                }
                            },
                            Flexibility = new()
                            {
                                Name = FlexibilityEnum.FLEX_1D.ToString()
                            },
                            VehicleSize = new()
                            {
                                Name = VehicleSizeEnum.SUV.ToString()
                            },
                            ScheduledAt = DateTime.Now.AddDays(1),
                            Status = new()
                            {
                                Name = StatusEnum.PENDING_APPROVAL.ToString()
                            },
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now,
                            DecisionAt = DateTime.MinValue,
                            Decision = null,
                            RequiresApproval = true,
                            Notes = "Test notes"
                        }
                    ]
                });

        _mockLinkService
            .Setup(x => x.GeneratePaginatedLinks(It.IsAny<GeneratePaginatedLinksDtoRequest>()))
            .Returns(new GeneratePaginatedLinksDtoResponse());

        _mockLinkService
            .Setup(u => u.GenerateSelf(It.IsAny<GenerateSelfLinkDtoRequest>()))
            .Returns($"https://api.test.com/bookings/{_mockBookingId}");

        // Act
        var result = await _bookingController.GetCustomerFilteredAsync(new()) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);

        var responseApi = (BookingApiPaginatedResponse)result.Value;
        Assert.Equal(1, responseApi.TotalItems);
        Assert.Equal(1, responseApi.TotalPages);
        Assert.Single(responseApi.Bookings);
        Assert.Equal($"https://api.test.com/bookings/{_mockBookingId}", responseApi.Bookings[0].Link.Self.Href);
    }

    [Fact]
    public async Task GetFilteredAsync_ShouldThrowArgumentNullException_WhenParamsAreNull()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _bookingController.GetFilteredAsync(null));
        Assert.Contains(Messages.InvalidRequestQueryParameters, exception.Message);
    }

    [Fact]
    public async Task GetFilteredAsync_ShouldReturnPagedResponse_WhenValidRequest()
    {
        // Arrange
        _claimsFixture.SetupUserClaims(_bookingController, Guid.Parse("00000000-0000-0000-0000-000000000099"));

        _mockBookingService
            .Setup(s => s.GetFilteredAsync(It.IsAny<BookingFilterDto>()))
            .ReturnsAsync(
                new BookingPaginatedDtoResponse
                {
                    TotalItems = 1,
                    TotalPages = 1,
                    Bookings =
                    [
                        new()
                        {
                            Id = _mockBookingId,
                            Reference = "REF123",
                            Customer = new()
                            {
                                ContactNumber = 1234567890,
                                Email = "test@example.com",
                                Role = new()
                                {
                                    Name = RoleEnum.USER.ToString()
                                }
                            },
                            Flexibility = new()
                            {
                                Name = FlexibilityEnum.FLEX_3D.ToString()
                            },
                            VehicleSize = new()
                            {
                                Name = VehicleSizeEnum.SEDAN.ToString()
                            },
                            ScheduledAt = DateTime.Now.AddDays(1),
                            Status = new()
                            {
                                Name = StatusEnum.PENDING_APPROVAL.ToString()
                            },
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now,
                            DecisionAt = DateTime.MinValue,
                            Decision = null,
                            RequiresApproval = true,
                            Notes = "Test notes"
                        }
                    ]
                });

        _mockLinkService
            .Setup(x => x.GeneratePaginatedLinks(It.IsAny<GeneratePaginatedLinksDtoRequest>()))
            .Returns(new GeneratePaginatedLinksDtoResponse());

        _mockLinkService
            .Setup(u => u.GenerateSelf(It.IsAny<GenerateSelfLinkDtoRequest>()))
            .Returns($"https://api.test.com/bookings/{_mockBookingId}");

        // Act
        var result = await _bookingController.GetFilteredAsync(new()) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);

        var responseApi = (BookingApiPaginatedResponse)result.Value;
        Assert.Equal(1, responseApi.TotalItems);
        Assert.Equal(1, responseApi.TotalPages);
        Assert.Single(responseApi.Bookings);
        Assert.Equal($"https://api.test.com/bookings/{_mockBookingId}", responseApi.Bookings[0].Link.Self.Href);
    }
}
