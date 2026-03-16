using Api.Controllers;
using Api.Models.Booking.Payload;
using Common.Enums;
using Common.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Service.Interfaces;
using Service.Models.Booking;
using Service.Models.Booking.Payload;
using Service.Models.Link.Payload;
using System.Net;
using System.Security.Claims;

namespace Api.Tests.Controllers;

public class BookingControllerTests
{
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
        SetupUserClaims(_bookingController, Guid.Parse("00000000-0000-0000-0000-000000000099"));

        _mockBookingService
            .Setup(s => s.CreateAsync(It.IsAny<BookingDto>()))
            .ReturnsAsync(_mockBookingId);

        // Act
        var result = await _bookingController.CreateAsync(
            new()
            {
                ScheduledAt = DateTime.Now.AddDays(1),
                FlexibilityId = _mockFlexibilityId,
                VehicleSizeId = _mockVehicleSizeId,
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
        SetupUserClaims(_bookingController, Guid.Parse("00000000-0000-0000-0000-000000000099"));

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
        SetupUserClaims(_bookingController, Guid.Parse("00000000-0000-0000-0000-000000000099"));

        _mockBookingService
            .Setup(s => s.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(
                new BookingDto()
                {
                    Id = _mockBookingId,
                    Reference = "REF123",
                    Customer = new()
                    {
                        Username = "username",
                        ContactNumber = 1234567890,
                        Email = "test@example.com",
                        Role = new()
                        {
                            Name = "CUSTOMER"
                        }
                    },
                    Flexibility = new()
                    {
                        Name = "1 Day"
                    },
                    VehicleSize = new()
                    {
                        Name = "SUV"
                    },
                    ScheduledAt = DateTime.Now.AddDays(1),
                    Status = new()
                    {
                        Name = "PENDING_APPROVAL"
                    },
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    DecisionAt = DateTime.MinValue,
                    Decision = null,
                    RequiresApproval = true,
                    Notes = "Test notes"
                });

        _mockLinkService.SetupSequence(u => u.GenerateSelf(It.IsAny<GenerateSelfLinkDtoRequest>()))
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
    public async Task GetFilteredAsync_ShouldThrowArgumentNullException_WhenParamsAreNull()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _bookingController.GetCustomerFilteredAsync(null));
        Assert.Contains(Messages.InvalidRequestQueryParameters, exception.Message);
    }

    [Fact]
    public async Task GetCustomerFilteredAsync_ShouldReturnPagedResponse_WhenValidRequest()
    {
        // Arrange
        SetupUserClaims(_bookingController, Guid.Parse("00000000-0000-0000-0000-000000000099"));

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
                                Username = "username",
                                ContactNumber = 1234567890,
                                Email = "test@example.com",
                                Role = new()
                                {
                                    Name = "CUSTOMER"
                                }
                            },
                            Flexibility = new()
                            {
                                Name = "1 Day"
                            },
                            VehicleSize = new()
                            {
                                Name = "SUV"
                            },
                            ScheduledAt = DateTime.Now.AddDays(1),
                            Status = new()
                            {
                                Name = "PENDING_APPROVAL"
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

        _mockLinkService.Setup(x => x.GeneratePaginatedLinks(It.IsAny<GeneratePaginatedLinksDtoRequest>()))
            .Returns(new GeneratePaginatedLinksDtoResponse());

        _mockLinkService.SetupSequence(u => u.GenerateSelf(It.IsAny<GenerateSelfLinkDtoRequest>()))
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

    private void SetupUserClaims(BookingController controller, Guid userId, string role = "ADMIN")
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Role, role)
        };

        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var principal = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext { User = principal };
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
    }
}
