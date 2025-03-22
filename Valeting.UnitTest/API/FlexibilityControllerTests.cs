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

namespace Valeting.Tests.API;

public class FlexibilityControllerTests
{
    private readonly string _mockFlexibilityId = "00000000-0000-0000-0000-000000000000";

    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IUrlService> _mockUrlService;
    private readonly Mock<IFlexibilityService> _mockFlexibilityService;

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
    public async Task GetFilteredAsync_ShouldReturnOk_WhenValidRequest()
    {
        // Arrange
        var dtoRequest = new PaginatedFlexibilityDtoRequest();
        _mockMapper.Setup(m => m.Map<PaginatedFlexibilityDtoRequest>(It.IsAny<FlexibilityApiParameters>())).Returns(dtoRequest);

        var dtoResponse = new PaginatedFlexibilityDtoResponse
        {
            TotalItems = 10,
            TotalPages = 2,
            Flexibilities = []
        };
        _mockFlexibilityService.Setup(s => s.GetFilteredAsync(It.IsAny<PaginatedFlexibilityDtoRequest>())).ReturnsAsync(dtoResponse);

        _mockUrlService.Setup(u => u.GeneratePaginatedLinks(It.IsAny<GeneratePaginatedLinksDtoRequest>())).Returns(new GeneratePaginatedLinksDtoResponse());
        
        var mappedLinks = new PaginationLinksApi();
        _mockMapper.Setup(m => m.Map<PaginationLinksApi>(It.IsAny<GeneratePaginatedLinksDtoResponse>())).Returns(mappedLinks);

        var mappedFlexibilities = new List<FlexibilityApi>()
        {
            new()
            {
                Id = Guid.Parse(_mockFlexibilityId),
                Description = It.IsAny<string>(),
                Active = It.IsAny<bool>()
            }
        };
        _mockMapper.Setup(m => m.Map<List<FlexibilityApi>>(dtoResponse.Flexibilities)).Returns(mappedFlexibilities);

        _mockUrlService.Setup(l => l.GenerateSelf(It.IsAny<GenerateSelfUrlDtoRequest>())).Returns(new GenerateSelfUrlDtoResponse());

        // Act
        var result = await _flexibilityController.GetFilteredAsync(new FlexibilityApiParameters { Active = false }) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
        var response = result.Value as FlexibilityApiPaginatedResponse;
        Assert.NotNull(response);
        Assert.Equal(1, response.CurrentPage);
        Assert.Equal(10, response.TotalItems);
        Assert.Equal(2, response.TotalPages);
    }

    [Fact]
    public async Task GetFilteredAsync_ShouldThrowArgumentNullException_WhenParamsAreNull()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _flexibilityController.GetFilteredAsync(null));
        Assert.Contains(Messages.InvalidRequestQueryParameters, exception.Message);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnOk_WhenValidId()
    {
        // Arrange
        var dtoResponse = new GetFlexibilityDtoResponse
        {
            Flexibility = new()
        };
        _mockFlexibilityService.Setup(s => s.GetByIdAsync(It.IsAny<GetFlexibilityDtoRequest>())).ReturnsAsync(dtoResponse);

        var mappedFlexibility = new FlexibilityApi();
        _mockMapper.Setup(m => m.Map<FlexibilityApi>(It.IsAny<FlexibilityDto>())).Returns(mappedFlexibility);

        var generateSelfResponse = new GenerateSelfUrlDtoResponse
        {
            Self = $"http://example.com/flexibility/{_mockFlexibilityId}"
        };
        _mockUrlService.Setup(u => u.GenerateSelf(It.IsAny<GenerateSelfUrlDtoRequest>())).Returns(generateSelfResponse);

        // Act
        var result = await _flexibilityController.GetByIdAsync(_mockFlexibilityId) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
        var response = result.Value as FlexibilityApiResponse;
        Assert.NotNull(response);
        Assert.NotNull(response.Flexibility);
        Assert.Equal(Guid.Parse(_mockFlexibilityId), response.Flexibility.Id);
        Assert.Equal($"http://example.com/flexibility/{_mockFlexibilityId}", response.Flexibility.Link.Self.Href);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowArgumentNullException_WhenIdIsNull()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _flexibilityController.GetByIdAsync(null));
        Assert.Contains(Messages.InvalidRequestId, exception.Message);
    }
}