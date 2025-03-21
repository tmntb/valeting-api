using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;
using Valeting.API.Controllers;
using Valeting.API.Models.Core;
using Valeting.API.Models.VehicleSize;
using Valeting.Common.Messages;
using Valeting.Common.Models.Link;
using Valeting.Common.Models.VehicleSize;
using Valeting.Core.Interfaces;

namespace Valeting.Tests.Api;

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
    public async Task GetFilteredAsync_ShouldReturnOk_WhenValidRequest()
    {
        // Arrange
        var dtoRequest = new PaginatedVehicleSizeDtoRequest();
        _mockMapper.Setup(m => m.Map<PaginatedVehicleSizeDtoRequest>(It.IsAny<VehicleSizeApiParameters>())).Returns(dtoRequest);

        var dtoResponse = new PaginatedVehicleSizeDtoResponse
        {
            TotalItems = 10,
            TotalPages = 2,
            VehicleSizes = []
        };
        _mockVehicleSizeService.Setup(s => s.GetFilteredAsync(It.IsAny<PaginatedVehicleSizeDtoRequest>())).ReturnsAsync(dtoResponse);

        _mockUrlService.Setup(u => u.GeneratePaginatedLinks(It.IsAny<GeneratePaginatedLinksDtoRequest>())).Returns(new GeneratePaginatedLinksDtoResponse());

        var mappedLinks = new PaginationLinksApi();
        _mockMapper.Setup(m => m.Map<PaginationLinksApi>(It.IsAny<GeneratePaginatedLinksDtoResponse>())).Returns(mappedLinks);

        var mappedVehicleSizes = new List<VehicleSizeApi>()
        {
            new()
            {
                Id = Guid.Parse(_mockVehicleSizeId),
                Description = It.IsAny<string>(),
                Active = It.IsAny<bool>()
            }
        };
        _mockMapper.Setup(m => m.Map<List<VehicleSizeApi>>(dtoResponse.VehicleSizes)).Returns(mappedVehicleSizes);

        _mockUrlService.Setup(l => l.GenerateSelf(It.IsAny<GenerateSelfUrlDtoRequest>())).Returns(new GenerateSelfUrlDtoResponse());

        // Act
        var result = await _vehicleSizeController.GetFilteredAsync(new VehicleSizeApiParameters { Active = false }) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
        var response = result.Value as VehicleSizeApiPaginatedResponse;
        Assert.NotNull(response);
        Assert.Equal(1, response.CurrentPage);
        Assert.Equal(10, response.TotalItems);
        Assert.Equal(2, response.TotalPages);
    }

    [Fact]
    public async Task GetFilteredAsync_ShouldThrowArgumentNullException_WhenParamsAreNull()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _vehicleSizeController.GetFilteredAsync(null));
        Assert.Contains(Messages.InvalidRequestQueryParameters, exception.Message);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnOk_WhenValidId()
    {
        // Arrange
        var dtoResponse = new GetVehicleSizeDtoResponse
        {
            VehicleSize = new()
        };
        _mockVehicleSizeService.Setup(s => s.GetByIdAsync(It.IsAny<GetVehicleSizeDtoRequest>())).ReturnsAsync(dtoResponse);

        var mappedVehicleSize = new VehicleSizeApi();
        _mockMapper.Setup(m => m.Map<VehicleSizeApi>(It.IsAny<VehicleSizeDto>())).Returns(mappedVehicleSize);

        var generateSelfResponse = new GenerateSelfUrlDtoResponse
        {
            Self = $"http://example.com/flexibility/{_mockVehicleSizeId}"
        };
        _mockUrlService.Setup(u => u.GenerateSelf(It.IsAny<GenerateSelfUrlDtoRequest>())).Returns(generateSelfResponse);

        // Act
        var result = await _vehicleSizeController.GetByIdAsync(_mockVehicleSizeId) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
        var response = result.Value as VehicleSizeApiResponse;
        Assert.NotNull(response);
        Assert.NotNull(response.VehicleSize);
        Assert.Equal(Guid.Parse(_mockVehicleSizeId), response.VehicleSize.Id);
        Assert.Equal($"http://example.com/flexibility/{_mockVehicleSizeId}", response.VehicleSize.Link.Self.Href);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowArgumentNullException_WhenIdIsNull()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _vehicleSizeController.GetByIdAsync(null));
        Assert.Contains(Messages.InvalidRequestId, exception.Message);
    }
}
