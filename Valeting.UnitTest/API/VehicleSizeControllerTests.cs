using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Net;
using Valeting.API.Controllers;
using Valeting.API.Models.Core;
using Valeting.API.Models.VehicleSize;
using Valeting.Common.Messages;
using Valeting.Common.Models.Link;
using Valeting.Common.Models.VehicleSize;
using Valeting.Core.Interfaces;

namespace Valeting.Tests.API;

public class VehicleSizeControllerTests
{
    private readonly string _mockVehicleSizeId = "00000000-0000-0000-0000-000000000000";

    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IUrlService> _mockUrlService;
    private readonly Mock<IVehicleSizeService> _mockVehicleSizeService;

    private readonly VehicleSizeController _vehicleSizeController;

    public VehicleSizeControllerTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockUrlService = new Mock<IUrlService>();
        _mockVehicleSizeService = new Mock<IVehicleSizeService>();

        _vehicleSizeController = new VehicleSizeController(_mockVehicleSizeService.Object, _mockUrlService.Object, _mockMapper.Object)
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
        _mockMapper.Setup(m => m.Map<PaginatedVehicleSizeDtoRequest>(It.IsAny<VehicleSizeApiParameters>())).Returns(new PaginatedVehicleSizeDtoRequest());
        _mockVehicleSizeService.Setup(s => s.GetFilteredAsync(It.IsAny<PaginatedVehicleSizeDtoRequest>())).ReturnsAsync(new PaginatedVehicleSizeDtoResponse
        {
            TotalItems = 1,
            TotalPages = 1,
            VehicleSizes = 
            [
                new()
                {
                    Id = It.IsAny<Guid>(),
                    Description = It.IsAny<string>(),
                    Active = It.IsAny<bool>()
                }    
            ]
        });
        _mockUrlService.Setup(u => u.GeneratePaginatedLinks(It.IsAny<GeneratePaginatedLinksDtoRequest>())).Returns(new GeneratePaginatedLinksDtoResponse());
        _mockMapper.Setup(m => m.Map<PaginationLinksApi>(It.IsAny<GeneratePaginatedLinksDtoResponse>())).Returns(new PaginationLinksApi());
        _mockMapper.Setup(m => m.Map<List<VehicleSizeApi>>(It.IsAny<List<VehicleSizeDto>>())).Returns(new List<VehicleSizeApi>
        {
            new()
            {
                Id = Guid.Parse(_mockVehicleSizeId),
                Description = It.IsAny<string>(),
                Active = It.IsAny<bool>()
            }
        });
        _mockUrlService.Setup(l => l.GenerateSelf(It.IsAny<GenerateSelfUrlDtoRequest>())).Returns(new GenerateSelfUrlDtoResponse
        {
            Self = $"https://api.test.com/vehicleSizes/{_mockVehicleSizeId}"
        });

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
        _mockVehicleSizeService.Setup(s => s.GetByIdAsync(It.IsAny<GetVehicleSizeDtoRequest>())).ReturnsAsync(new GetVehicleSizeDtoResponse());
        _mockMapper.Setup(m => m.Map<VehicleSizeApi>(It.IsAny<VehicleSizeDto>())).Returns(new VehicleSizeApi());
        _mockUrlService.Setup(u => u.GenerateSelf(It.IsAny<GenerateSelfUrlDtoRequest>())).Returns(new GenerateSelfUrlDtoResponse
        {
            Self = $"http://example.com/flexibility/{_mockVehicleSizeId}"
        });

        // Act
        var result = await _vehicleSizeController.GetByIdAsync(_mockVehicleSizeId) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);

        var responseApi = result.Value as VehicleSizeApiResponse;
        Assert.NotNull(responseApi);
        Assert.NotNull(responseApi.VehicleSize);
        Assert.Equal(Guid.Parse(_mockVehicleSizeId), responseApi.VehicleSize.Id);
        Assert.Equal($"http://example.com/flexibility/{_mockVehicleSizeId}", responseApi.VehicleSize.Link.Self.Href);
    }
}
