using Api.Controllers;
using Api.Models.VehicleSize;
using Common.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Service.Interfaces;
using Service.Models.Link.Payload;
using Service.Models.VehicleSize;
using Service.Models.VehicleSize.Payload;
using System.Net;

namespace Api.Tests.Controllers;

public class VehicleSizeControllerTests
{
    private readonly Mock<ILinkService> _mockLinkService;
    private readonly Mock<IVehicleSizeService> _mockVehicleSizeService;

    private readonly Guid _mockVehicleSizeId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private readonly VehicleSizeController _vehicleSizeController;

    public VehicleSizeControllerTests()
    {
        _mockLinkService = new Mock<ILinkService>();
        _mockVehicleSizeService = new Mock<IVehicleSizeService>();

        _vehicleSizeController = new VehicleSizeController(_mockVehicleSizeService.Object, _mockLinkService.Object)
        {
            ControllerContext = new() { HttpContext = new DefaultHttpContext() }
        };
    }

    [Fact]
    public async Task GetFilteredAsync_ShouldThrowArgumentNullException_WhenParamsAreNull()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _vehicleSizeController.GetFilteredAsync(null));
        Assert.Contains(Messages.InvalidRequestQueryParameters, exception.Message);
    }

    [Fact]
    public async Task GetFilteredAsync_ShouldReturnOk_WhenValidRequest()
    {
        // Arrange
        _mockVehicleSizeService.Setup(s => s.GetFilteredAsync(It.IsAny<VehicleSizeFilterDto>()))
            .ReturnsAsync(
                new VehicleSizePaginatedDtoResponse
                {
                    TotalItems = 1,
                    TotalPages = 1,
                    VehicleSizes =
                    [
                        new()
                        {
                            Id = _mockVehicleSizeId,
                            Description = "description",
                            Active = true
                        }
                    ]
                });

        _mockLinkService.Setup(u => u.GeneratePaginatedLinks(It.IsAny<GeneratePaginatedLinksDtoRequest>()))
            .Returns(new GeneratePaginatedLinksDtoResponse());

        _mockLinkService.Setup(l => l.GenerateSelf(It.IsAny<GenerateSelfLinkDtoRequest>()))
            .Returns($"https://api.test.com/vehicleSizes/{_mockVehicleSizeId}");

        // Act
        var result = await _vehicleSizeController.GetFilteredAsync
        (
            new()
            {
                Active = false
            }
        ) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);

        var responseApi = result.Value as VehicleSizeApiPaginatedResponse;
        Assert.NotNull(responseApi);
        Assert.Equal(1, responseApi.CurrentPage);
        Assert.Equal(1, responseApi.TotalItems);
        Assert.Equal(1, responseApi.TotalPages);
        Assert.Equal($"https://api.test.com/vehicleSizes/{_mockVehicleSizeId}", responseApi.VehicleSizes[0].Link.Self.Href);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowArgumentNullException_WhenIdIsNull()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _vehicleSizeController.GetByIdAsync(null));
        Assert.Contains(Messages.InvalidRequestId, exception.Message);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnOk_WhenValidId()
    {
        // Arrange
        _mockVehicleSizeService.Setup(s => s.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(
                new VehicleSizeDto
                {
                    Id = _mockVehicleSizeId
                });

        _mockLinkService.Setup(u => u.GenerateSelf(It.IsAny<GenerateSelfLinkDtoRequest>()))
            .Returns($"http://example.com/flexibility/{_mockVehicleSizeId}");

        // Act
        var result = await _vehicleSizeController.GetByIdAsync(_mockVehicleSizeId.ToString()) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);

        var responseApi = result.Value as VehicleSizeApiResponse;
        Assert.NotNull(responseApi);
        Assert.NotNull(responseApi.VehicleSize);
        Assert.Equal(_mockVehicleSizeId, responseApi.VehicleSize.Id);
        Assert.Equal($"http://example.com/flexibility/{_mockVehicleSizeId}", responseApi.VehicleSize.Link.Self.Href);
    }
}
