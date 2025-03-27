using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;
using Valeting.API.Controllers;
using Valeting.API.Models.Booking;
using Valeting.API.Models.Core;
using Valeting.Common.Messages;
using Valeting.Common.Models.Booking;
using Valeting.Common.Models.Link;
using Valeting.Core.Interfaces;

namespace Valeting.Tests.API.Controllers;

public class BookingControllerTests
{
    private readonly Mock<IBookingService> _mockBookingService;
    private readonly Mock<IUrlService> _mockUrlService;
    private readonly Mock<IMapper> _mockMapper;

    private readonly Guid _mockBookingId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private readonly Guid _mockFlexibilityId = Guid.Parse("00000000-0000-0000-0000-000000000002");
    private readonly Guid _mockVehicleSizeId = Guid.Parse("00000000-0000-0000-0000-000000000003");
    private readonly BookingController _bookingController;

    public BookingControllerTests()
    {
        _mockBookingService = new Mock<IBookingService>();
        _mockUrlService = new Mock<IUrlService>();
        _mockMapper = new Mock<IMapper>();

        _bookingController = new BookingController(_mockBookingService.Object, _mockUrlService.Object, _mockMapper.Object);
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
        _mockMapper.Setup(m => m.Map<CreateBookingDtoRequest>(It.IsAny<CreateBookingApiRequest>()))
            .Returns(new CreateBookingDtoRequest());

        _mockBookingService.Setup(s => s.CreateAsync(It.IsAny<CreateBookingDtoRequest>()))
            .ReturnsAsync(new CreateBookingDtoResponse());

        _mockMapper.Setup(m => m.Map<CreateBookingApiResponse>(It.IsAny<CreateBookingDtoResponse>()))
            .Returns(
                new CreateBookingApiResponse()
                {
                    Id = _mockBookingId
                });

        // Act
        var result = await _bookingController.CreateAsync(
            new()
            {
                Name = "name",
                BookingDate = DateTime.Now.AddDays(1),
                Flexibility = new(),
                VehicleSize = new(),
                ContactNumber = 123,
                Email = "email"
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
        _mockMapper.Setup(m => m.Map<UpdateBookingDtoRequest>(It.IsAny<UpdateBookingApiRequest>()))
            .Returns(
                new UpdateBookingDtoRequest
                {
                    Id = _mockBookingId
                });

        _mockBookingService.Setup(s => s.UpdateAsync(It.IsAny<UpdateBookingDtoRequest>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _bookingController.UpdateAsync
        (
            _mockBookingId.ToString(),
            new()
            {
                Name = "name",
                BookingDate = DateTime.Now.AddDays(1),
                Flexibility = new(),
                VehicleSize = new(),
                ContactNumber = 123,
                Email = "email",
                Approved = true
            }
        ) as StatusCodeResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.NoContent, result.StatusCode);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowArgumentNullException_WhenIdIsNull()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _bookingController.DeleteAsync(null));
        Assert.Contains(Messages.InvalidRequestId, exception.Message);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnNoContent_WhenValidId()
    {
        // Arrange
        _mockBookingService.Setup(s => s.DeleteAsync(It.IsAny<DeleteBookingDtoRequest>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _bookingController.DeleteAsync(_mockBookingId.ToString()) as StatusCodeResult;

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
        _mockBookingService.Setup(s => s.GetByIdAsync(It.IsAny<GetBookingDtoRequest>()))
            .ReturnsAsync(
                new GetBookingDtoResponse
                {
                    Booking = new()
                    {
                        Id = _mockBookingId
                    }
                });

        _mockMapper.Setup(m => m.Map<BookingApi>(It.IsAny<BookingDto>()))
            .Returns(
                 new BookingApi
                 {
                     Id = _mockBookingId,
                     Name = "name",
                     BookingDate = DateTime.Now.AddDays(1),
                     Flexibility = new()
                     {
                         Id = _mockFlexibilityId
                     },
                     VehicleSize = new()
                     {
                         Id = _mockVehicleSizeId
                     },
                     ContactNumber = 123,
                     Email = "email",
                     Approved = true
                 });

        _mockUrlService.SetupSequence(u => u.GenerateSelf(It.IsAny<GenerateSelfUrlDtoRequest>()))
            .Returns(new GenerateSelfUrlDtoResponse { Self = $"https://api.test.com/flexibilities/{_mockFlexibilityId}" })
            .Returns(new GenerateSelfUrlDtoResponse { Self = $"https://api.test.com/vehicleSizes/{_mockVehicleSizeId}" })
            .Returns(new GenerateSelfUrlDtoResponse { Self = $"https://api.test.com/bookings/{_mockBookingId}" });

        // Act
        var result = await _bookingController.GetByIdAsync(_mockBookingId.ToString()) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);

        var responseApi = (BookingApiResponse)result.Value;
        Assert.Equal(_mockBookingId, responseApi.Booking.Id);
        Assert.Equal($"https://api.test.com/bookings/{_mockBookingId}", responseApi.Booking.Link.Self.Href);
        Assert.Equal($"https://api.test.com/flexibilities/{_mockFlexibilityId}", responseApi.Booking.Flexibility.Link.Self.Href);
        Assert.Equal($"https://api.test.com/vehicleSizes/{_mockVehicleSizeId}", responseApi.Booking.VehicleSize.Link.Self.Href);
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
        _mockMapper.Setup(m => m.Map<PaginatedBookingDtoRequest>(It.IsAny<BookingApiParameters>()))
            .Returns(new PaginatedBookingDtoRequest());

        _mockBookingService.Setup(s => s.GetFilteredAsync(It.IsAny<PaginatedBookingDtoRequest>()))
            .ReturnsAsync(
                new PaginatedBookingDtoResponse 
                { 
                    TotalItems = 1,
                    TotalPages = 1,
                    Bookings = [] 
                });

        _mockUrlService.Setup(x => x.GeneratePaginatedLinks(It.IsAny<GeneratePaginatedLinksDtoRequest>()))
            .Returns(new GeneratePaginatedLinksDtoResponse());

        _mockMapper.Setup(s => s.Map<PaginationLinksApi>(It.IsAny<GeneratePaginatedLinksDtoResponse>()))
            .Returns(new PaginationLinksApi());

        _mockMapper.Setup(m => m.Map<List<BookingApi>>(It.IsAny<List<BookingDto>>()))
            .Returns(
                [
                    new()
                    {
                        Id = _mockBookingId,
                        Name = "name",
                        BookingDate = DateTime.Now.AddDays(1),
                        Flexibility = new()
                        {
                            Id = _mockFlexibilityId
                        },
                        VehicleSize = new()
                        {
                            Id = _mockVehicleSizeId
                        },
                        ContactNumber = 123,
                        Email = "email",
                        Approved = true
                    }
                ]);

        _mockUrlService.SetupSequence(u => u.GenerateSelf(It.IsAny<GenerateSelfUrlDtoRequest>()))
            .Returns(new GenerateSelfUrlDtoResponse { Self = $"https://api.test.com/flexibilities/{_mockFlexibilityId}" })
            .Returns(new GenerateSelfUrlDtoResponse { Self = $"https://api.test.com/vehicleSizes/{_mockVehicleSizeId}" })
            .Returns(new GenerateSelfUrlDtoResponse { Self = $"https://api.test.com/bookings/{_mockBookingId}" });

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
        Assert.Equal($"https://api.test.com/flexibilities/{_mockFlexibilityId}", responseApi.Bookings[0].Flexibility.Link.Self.Href);
        Assert.Equal($"https://api.test.com/vehicleSizes/{_mockVehicleSizeId}", responseApi.Bookings[0].VehicleSize.Link.Self.Href);
    }
}
