using Api.Controllers;
using Api.Models.Flexibility.Payload;
using Common.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Service.Interfaces;
using Service.Models.Flexibility;
using Service.Models.Flexibility.Payload;
using Service.Models.Link.Payload;
using System.Net;

namespace Api.Tests.Controllers;

public class FlexibilityControllerTests
{
    private readonly Mock<ILinkService> _mockLinkService;
    private readonly Mock<IFlexibilityService> _mockFlexibilityService;

    private readonly Guid _mockFlexibilityId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private readonly FlexibilityController _flexibilityController;

    public FlexibilityControllerTests()
    {
        _mockLinkService = new Mock<ILinkService>();
        _mockFlexibilityService = new Mock<IFlexibilityService>();

        _flexibilityController = new FlexibilityController(_mockFlexibilityService.Object, _mockLinkService.Object)
        {
            ControllerContext = new() { HttpContext = new DefaultHttpContext() }
        };
    }

    [Fact]
    public async Task GetFilteredAsync_ShouldThrowArgumentNullException_WhenParamsAreNull()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _flexibilityController.GetFilteredAsync(null));
        Assert.Contains(Messages.InvalidRequestQueryParameters, exception.Message);
    }

    [Fact]
    public async Task GetFilteredAsync_ShouldReturnOk_WhenValidRequest()
    {
        // Arrange
        _mockFlexibilityService.Setup(s => s.GetFilteredAsync(It.IsAny<FlexibilityFilterDto>()))
            .ReturnsAsync(
                new FlexibilityPaginatedDtoResponse
                {
                    TotalItems = 1,
                    TotalPages = 1,
                    Flexibilities =
                    [
                        new()
                        {
                            Id = It.IsAny<Guid>(),
                            Description = It.IsAny<string>(),
                            Active = It.IsAny<bool>()
                        }
                    ]
                });

        _mockLinkService.Setup(u => u.GeneratePaginatedLinks(It.IsAny<GeneratePaginatedLinksDtoRequest>()))
            .Returns(new GeneratePaginatedLinksDtoResponse());

        _mockLinkService.Setup(l => l.GenerateSelf(It.IsAny<GenerateSelfLinkDtoRequest>()))
            .Returns($"https://api.test.com/flexibilities/{_mockFlexibilityId}");

        // Act
        var result = await _flexibilityController.GetFilteredAsync
        (
            new()
            {
                Active = false
            }
        ) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);

        var responseApi = result.Value as FlexibilityApiPaginatedResponse;
        Assert.NotNull(responseApi);
        Assert.Equal(1, responseApi.CurrentPage);
        Assert.Equal(1, responseApi.TotalItems);
        Assert.Equal(1, responseApi.TotalPages);
        Assert.Equal($"https://api.test.com/flexibilities/{_mockFlexibilityId}", responseApi.Flexibilities[0].Link.Self.Href);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowArgumentNullException_WhenIdIsNull()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _flexibilityController.GetByIdAsync(null));
        Assert.Contains(Messages.InvalidRequestId, exception.Message);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnOk_WhenValidId()
    {
        // Arrange
        _mockFlexibilityService.Setup(s => s.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(
                new FlexibilityDto
                {
                    Id = _mockFlexibilityId
                });

        _mockLinkService.Setup(u => u.GenerateSelf(It.IsAny<GenerateSelfLinkDtoRequest>()))
            .Returns($"http://example.com/flexibility/{_mockFlexibilityId}");

        // Act
        var result = await _flexibilityController.GetByIdAsync(_mockFlexibilityId.ToString()) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);

        var responseApi = result.Value as FlexibilityApiResponse;
        Assert.NotNull(responseApi);
        Assert.NotNull(responseApi.Flexibility);
        Assert.Equal(_mockFlexibilityId, responseApi.Flexibility.Id);
        Assert.Equal($"http://example.com/flexibility/{_mockFlexibilityId}", responseApi.Flexibility.Link.Self.Href);
    }
}