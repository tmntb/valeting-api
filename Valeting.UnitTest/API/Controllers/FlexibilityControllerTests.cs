using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;
using Valeting.API.Controllers;
using Valeting.API.Models.Core;
using Valeting.API.Models.Flexibility;
using Valeting.Common.Messages;
using Valeting.Common.Models.Flexibility;
using Valeting.Common.Models.Link;
using Valeting.Core.Interfaces;

namespace Valeting.Tests.API.Controllers;

public class FlexibilityControllerTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IUrlService> _mockUrlService;
    private readonly Mock<IFlexibilityService> _mockFlexibilityService;

    private readonly Guid _mockFlexibilityId = Guid.Parse( "00000000-0000-0000-0000-000000000001");
    private readonly FlexibilityController _flexibilityController;

    public FlexibilityControllerTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockUrlService = new Mock<IUrlService>();
        _mockFlexibilityService = new Mock<IFlexibilityService>();

        _flexibilityController = new FlexibilityController(_mockFlexibilityService.Object, _mockUrlService.Object, _mockMapper.Object)
        {
            ControllerContext = new() {  HttpContext = new DefaultHttpContext() }
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
        _mockMapper.Setup(m => m.Map<PaginatedFlexibilityDtoRequest>(It.IsAny<FlexibilityApiParameters>()))
            .Returns(new PaginatedFlexibilityDtoRequest());

        _mockFlexibilityService.Setup(s => s.GetFilteredAsync(It.IsAny<PaginatedFlexibilityDtoRequest>()))
            .ReturnsAsync(
                new PaginatedFlexibilityDtoResponse
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

        _mockUrlService.Setup(u => u.GeneratePaginatedLinks(It.IsAny<GeneratePaginatedLinksDtoRequest>()))
            .Returns(new GeneratePaginatedLinksDtoResponse());

        _mockMapper.Setup(m => m.Map<PaginationLinksApi>(It.IsAny<GeneratePaginatedLinksDtoResponse>()))
            .Returns(new PaginationLinksApi());

        _mockMapper.Setup(m => m.Map<List<FlexibilityApi>>(It.IsAny<List<FlexibilityDto>>()))
            .Returns(new List<FlexibilityApi>
            {
                new()
                {
                    Id = It.IsAny<Guid>(),
                    Description = It.IsAny<string>(),
                    Active = It.IsAny<bool>()
                }
            });
        
        _mockUrlService.Setup(l => l.GenerateSelf(It.IsAny<GenerateSelfUrlDtoRequest>()))
            .Returns(
                new GenerateSelfUrlDtoResponse
                {
                    Self = $"https://api.test.com/flexibilities/{_mockFlexibilityId}"
                });

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
        _mockFlexibilityService.Setup(s => s.GetByIdAsync(It.IsAny<GetFlexibilityDtoRequest>()))
            .ReturnsAsync(
                new GetFlexibilityDtoResponse
                {
                    Flexibility = new()
                    {
                        Id = _mockFlexibilityId
                    }
                });

        _mockMapper.Setup(m => m.Map<FlexibilityApi>(It.IsAny<FlexibilityDto>()))
            .Returns(
                new FlexibilityApi
                {
                    Id = _mockFlexibilityId
                });

        _mockUrlService.Setup(u => u.GenerateSelf(It.IsAny<GenerateSelfUrlDtoRequest>()))
            .Returns(
                new GenerateSelfUrlDtoResponse
                {
                    Self = $"http://example.com/flexibility/{_mockFlexibilityId}"
                });

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