using AutoMapper;

using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using Valeting.Cache.Interfaces;
using Valeting.Core.Services.Interfaces;
using Valeting.Core.Models.Flexibility;
using Valeting.Models.Flexibility;
using Valeting.Controllers;
using Valeting.Core.Models.Link;
using Valeting.Models.Core;

namespace Valeting.UnitTest.API;

public class FlexibilityControllerTests
{
    private Mock<IMapper> _mapperMock;
    private Mock<HttpRequest> _httpRequest;
    private Mock<IUrlService> _urlServiceMock;
    private Mock<ICacheHandler> _cacheHandlerMock;
    private Mock<IFlexibilityService> _flexibilityServiceMock;

    public FlexibilityControllerTests()
    {
        _mapperMock = new Mock<IMapper>();
        _httpRequest = new Mock<HttpRequest>();
        _urlServiceMock = new Mock<IUrlService>();
        _cacheHandlerMock = new Mock<ICacheHandler>();
        _flexibilityServiceMock = new Mock<IFlexibilityService>();
    }

    [Fact]
    public async Task GetById_Status200_WithoutCache()
    {
        _httpRequest.Setup(x => x.Host).Returns(HostString.FromUriComponent("http://localhost:8080"));
        _httpRequest.Setup(x => x.Path).Returns(PathString.FromUriComponent("/flexibilities/{0}"));

        var httpContext = Mock.Of<HttpContext>(x => x.Request == _httpRequest.Object);
        var controllerContext = new ControllerContext()
        {
            HttpContext = httpContext
        };

        var id = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var getFlexibilitySVResponse = new GetFlexibilitySVResponse
        {
            Flexibility = new()
            {
                Id = id, 
                Description = "flex",
                Active = true
            }
        };
        _flexibilityServiceMock.Setup(x => x.GetByIdAsync(It.IsAny<GetFlexibilitySVRequest>())).ReturnsAsync(getFlexibilitySVResponse);

        var flexibilityApi = new FlexibilityApi
        {
            Id = id,
            Description = "flex",
            Active = true
        };
        _mapperMock.Setup(x => x.Map<FlexibilityApi>(It.IsAny<FlexibilitySV>())).Returns(flexibilityApi);

        var generateSelfUrlSVResponse = new GenerateSelfUrlSVResponse() { Self = string.Format("https://localhost:8080/Valeting/flexibilities/{0}", id) };
        _urlServiceMock.Setup(x => x.GenerateSelf(It.IsAny<GenerateSelfUrlSVRequest>())).Returns(generateSelfUrlSVResponse);

        // Act
        var flexibilityController = new FlexibilityController(_flexibilityServiceMock.Object, _urlServiceMock.Object, _cacheHandlerMock.Object, _mapperMock.Object)
        {
            ControllerContext = controllerContext
        };

        var response = await flexibilityController.GetByIdAsync(id.ToString());

        // Assert
        Assert.NotNull(response);

        var okResult = response as ObjectResult;
        Assert.NotNull(okResult);
        Assert.Equal(200, okResult.StatusCode);

        var flexibilityApiResponse = okResult.Value as FlexibilityApiResponse;
        Assert.NotNull(flexibilityApiResponse);
        Assert.Equal(id, flexibilityApiResponse.Flexibility.Id);
        Assert.Equal("flex", flexibilityApiResponse.Flexibility.Description);
        Assert.True(flexibilityApiResponse.Flexibility.Active);
        Assert.NotNull(flexibilityApiResponse.Flexibility.Link.Self);
        Assert.False(string.IsNullOrEmpty(flexibilityApiResponse.Flexibility.Link.Self.Href));
        Assert.Contains(string.Format("/Valeting/flexibilities/{0}", id), flexibilityApiResponse.Flexibility.Link.Self.Href);
    }

    [Fact]
    public async Task GetById_Status200_WithoutCache_WithError()
    {
        _httpRequest.Setup(x => x.Host).Returns(HostString.FromUriComponent("http://localhost:8080"));
        _httpRequest.Setup(x => x.Path).Returns(PathString.FromUriComponent("/flexibilities/{0}"));

        var httpContext = Mock.Of<HttpContext>(x => x.Request == _httpRequest.Object);
        var controllerContext = new ControllerContext()
        {
            HttpContext = httpContext
        };

        var id = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var getFlexibilitySVResponse = new GetFlexibilitySVResponse
        {
            Error = new() 
            {
                ErrorCode = 404, 
                Message = "NotFound" 
            }
        };
        _flexibilityServiceMock.Setup(x => x.GetByIdAsync(It.IsAny<GetFlexibilitySVRequest>())).ReturnsAsync(getFlexibilitySVResponse);

        // Act
        var flexibilityController = new FlexibilityController(_flexibilityServiceMock.Object, _urlServiceMock.Object, _cacheHandlerMock.Object, _mapperMock.Object)
        {
            ControllerContext = controllerContext
        };

        var response = await flexibilityController.GetByIdAsync(id.ToString());

        // Assert
        Assert.NotNull(response);

        var notFoundResult = response as ObjectResult;
        Assert.NotNull(notFoundResult);
        Assert.Equal(404, notFoundResult.StatusCode);

        var flexibilityApiResponse = notFoundResult.Value as FlexibilityApiError;
        Assert.NotNull(flexibilityApiResponse);
        Assert.Equal("NotFound", flexibilityApiResponse.Detail);
    }

    [Fact]
    public async Task GetById_Status200_WithCache()
    {
        _httpRequest.Setup(x => x.Host).Returns(HostString.FromUriComponent("http://localhost:8080"));
        _httpRequest.Setup(x => x.Path).Returns(PathString.FromUriComponent("/flexibilities/{0}"));

        var httpContext = Mock.Of<HttpContext>(x => x.Request == _httpRequest.Object);
        var controllerContext = new ControllerContext()
        {
            HttpContext = httpContext
        };

        var id = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var getFlexibilitySVResponse = new GetFlexibilitySVResponse
        {
            Flexibility = new()
            {
                Id = id, 
                Description = "flex",
                Active = true
            }
        };
        _cacheHandlerMock.Setup(x => x.GetRecord<GetFlexibilitySVResponse>(It.IsAny<string>())).Returns(getFlexibilitySVResponse);

        var flexibilityApi = new FlexibilityApi
        {
            Id = id,
            Description = "flex",
            Active = true
        };
        _mapperMock.Setup(x => x.Map<FlexibilityApi>(It.IsAny<FlexibilitySV>())).Returns(flexibilityApi);

        var generateSelfUrlSVResponse = new GenerateSelfUrlSVResponse() { Self = string.Format("https://localhost:8080/Valeting/flexibilities/{0}", id) };
        _urlServiceMock.Setup(x => x.GenerateSelf(It.IsAny<GenerateSelfUrlSVRequest>())).Returns(generateSelfUrlSVResponse);

        // Act
        var flexibilityController = new FlexibilityController(_flexibilityServiceMock.Object, _urlServiceMock.Object, _cacheHandlerMock.Object, _mapperMock.Object)
        {
            ControllerContext = controllerContext
        };

        var response = await flexibilityController.GetByIdAsync(id.ToString());

        // Assert
        Assert.NotNull(response);

        var okResult = response as ObjectResult;
        Assert.NotNull(okResult);
        Assert.Equal(200, okResult.StatusCode);

        var flexibilityApiResponse = okResult.Value as FlexibilityApiResponse;
        Assert.NotNull(flexibilityApiResponse);
        Assert.Equal(id, flexibilityApiResponse.Flexibility.Id);
        Assert.Equal("flex", flexibilityApiResponse.Flexibility.Description);
        Assert.True(flexibilityApiResponse.Flexibility.Active);
        Assert.NotNull(flexibilityApiResponse.Flexibility.Link.Self);
        Assert.False(string.IsNullOrEmpty(flexibilityApiResponse.Flexibility.Link.Self.Href));
        Assert.Contains(string.Format("/Valeting/flexibilities/{0}", id), flexibilityApiResponse.Flexibility.Link.Self.Href);
    }

    [Fact]
    public async Task GetById_Status500_WithException()
    {
        // Act
        var flexibilityController = new FlexibilityController(_flexibilityServiceMock.Object, _urlServiceMock.Object, _cacheHandlerMock.Object, _mapperMock.Object);
        var response = await flexibilityController.GetByIdAsync(null);

        // Assert
        Assert.NotNull(response);

        var internalServerErrorResult = response as ObjectResult;
        Assert.NotNull(internalServerErrorResult);
        Assert.Equal(500, internalServerErrorResult.StatusCode);

        var flexibilityApiResponse = internalServerErrorResult.Value as FlexibilityApiError;
        Assert.NotNull(flexibilityApiResponse);
        Assert.False(string.IsNullOrEmpty(flexibilityApiResponse.Detail));
    }

    [Fact]
    public async Task Get_Status200_WithoutCache()
    {
        // Arrange
        _httpRequest.Setup(x => x.Host).Returns(HostString.FromUriComponent("http://localhost:8080"));
        _httpRequest.Setup(x => x.Path).Returns(PathString.FromUriComponent("/Valeting/flexibilities"));

        var httpContext = Mock.Of<HttpContext>(x => x.Request == _httpRequest.Object);
        var controllerContext = new ControllerContext()
        {
            HttpContext = httpContext
        };

        var flexibilityApiParameters = new FlexibilityApiParameters
        {
            PageNumber = 1,
            PageSize = 2,
            Active = true
        };

        var paginatedFlexibilitySVRequest = new PaginatedFlexibilitySVRequest
        {
            Filter = new()
            {
                PageNumber = 1,
                PageSize = 2,
                Active = true
            }
        };
        _mapperMock.Setup(x => x.Map<PaginatedFlexibilitySVRequest>(flexibilityApiParameters)).Returns(paginatedFlexibilitySVRequest);

        var flexibilitiesSV_List = new List<FlexibilitySV>()
        {
            new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), Description = "flex1", Active = true },
            new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000003"), Description = "flex2", Active = true },
            new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000004"), Description = "flex3", Active = true }
        };

        var paginatedFlexibilitySVResponse = new PaginatedFlexibilitySVResponse
        {
            Flexibilities = flexibilitiesSV_List,
            TotalItems = 3,
            TotalPages = 1
        };
        _flexibilityServiceMock.Setup(x => x.GetAsync(It.IsAny<PaginatedFlexibilitySVRequest>())).ReturnsAsync(paginatedFlexibilitySVResponse);

        var paginatedLinks = new GeneratePaginatedLinksSVResponse
        {
            Next = string.Format("https://localhost:8080/Valeting/flexibilities?pageNumber={0}&pageSize={1}", 1, 2),
            Prev = string.Empty,
            Self = string.Format("https://localhost:8080/Valeting/flexibilities?pageNumber={0}&pageSize={1}", 1, 2)
        };
        _urlServiceMock.Setup(x => x.GeneratePaginatedLinks(It.IsAny<GeneratePaginatedLinksSVRequest>())).Returns(paginatedLinks);

        var paginationLinksApi = new PaginationLinksApi
        {
            Next = new() { Href = string.Format("https://localhost:8080/Valeting/flexibilities?pageNumber={0}&pageSize={1}", 1, 2) },
            Prev = new() { Href = string.Empty },
            Self = new() { Href = string.Format("https://localhost:8080/Valeting/flexibilities?pageNumber={0}&pageSize={1}", 1, 2) }
        };
        _mapperMock.Setup(x => x.Map<PaginationLinksApi>(paginatedLinks)).Returns(paginationLinksApi);

        var flexibilityApi_List = flexibilitiesSV_List.Select(vs => new FlexibilityApi
        {
            Id = vs.Id,
            Description = vs.Description,
            Active = vs.Active,
            Link = new()
            {
                Self = new()
                {
                    Href = string.Format("https://localhost:8080/Valeting/flexibilities/{0}", vs.Id)
                }
            }
        }).ToList();
        _mapperMock.Setup(x => x.Map<List<FlexibilityApi>>(flexibilitiesSV_List)).Returns(flexibilityApi_List);

        flexibilitiesSV_List.ForEach(x =>
        {
            var generateSelfUrlSVResponse = new GenerateSelfUrlSVResponse() { Self = string.Format("https://localhost:8080/Valeting/flexbilities/{0}", x.Id) };
            _urlServiceMock.Setup(x => x.GenerateSelf(It.IsAny<GenerateSelfUrlSVRequest>())).Returns(generateSelfUrlSVResponse);
        });

        // Act
        var flexibilityController = new FlexibilityController(_flexibilityServiceMock.Object, _urlServiceMock.Object, _cacheHandlerMock.Object, _mapperMock.Object)
        {
            ControllerContext = controllerContext
        };

        var response = await flexibilityController.GetAsync(flexibilityApiParameters);

        // Assert
        Assert.NotNull(response);

        var okResult = response as ObjectResult;
        Assert.NotNull(okResult);
        Assert.Equal(200, okResult.StatusCode);

        var flexibilityApiResponse = okResult.Value as FlexibilityApiPaginatedResponse;
        Assert.NotNull(flexibilityApiResponse);
        Assert.NotEmpty(flexibilityApiResponse.Flexibilities);
        Assert.Equal(3, flexibilityApiResponse.Flexibilities.Count);
        Assert.Equal(1, flexibilityApiResponse.TotalPages);
        Assert.Equal(3, flexibilityApiResponse.TotalItems);
        Assert.NotNull(flexibilityApiResponse.Links);
        Assert.Contains(string.Format("/Valeting/flexibilities?pageNumber={0}&pageSize={1}", 1, 2), flexibilityApiResponse.Links.Next.Href);
        Assert.Contains(string.Format("/Valeting/flexibilities?pageNumber={0}&pageSize={1}", 1, 2), flexibilityApiResponse.Links.Self.Href);
        Assert.Contains(string.Empty, flexibilityApiResponse.Links.Prev.Href);
    }

    [Fact]
    public async Task Get_Status200_WithCache()
    {
        // Arrange
        _httpRequest.Setup(x => x.Host).Returns(HostString.FromUriComponent("http://localhost:8080"));
        _httpRequest.Setup(x => x.Path).Returns(PathString.FromUriComponent("/Valeting/flexibilities"));

        var httpContext = Mock.Of<HttpContext>(x => x.Request == _httpRequest.Object);
        var controllerContext = new ControllerContext()
        {
            HttpContext = httpContext
        };

        var flexibilityApiParameters = new FlexibilityApiParameters
        {
            PageNumber = 1,
            PageSize = 2,
            Active = true
        };

        var paginatedFlexibilitySVRequest = new PaginatedFlexibilitySVRequest
        {
            Filter = new()
            {
                PageNumber = 1,
                PageSize = 2,
                Active = true
            }
        };
        _mapperMock.Setup(x => x.Map<PaginatedFlexibilitySVRequest>(flexibilityApiParameters)).Returns(paginatedFlexibilitySVRequest);

        var flexibilitiesSV_List = new List<FlexibilitySV>()
        {
            new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), Description = "flex1", Active = true },
            new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000003"), Description = "flex2", Active = true },
            new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000004"), Description = "flex3", Active = true }
        };

        var paginatedFlexibilitySVResponse = new PaginatedFlexibilitySVResponse
        {
            Flexibilities = flexibilitiesSV_List,
            TotalItems = 3,
            TotalPages = 1
        };
        _cacheHandlerMock.Setup(x => x.GetRecord<PaginatedFlexibilitySVResponse>(It.IsAny<string>())).Returns(paginatedFlexibilitySVResponse);

        var paginatedLinks = new GeneratePaginatedLinksSVResponse
        {
            Next = string.Format("https://localhost:8080/Valeting/flexibilities?pageNumber={0}&pageSize={1}", 1, 2),
            Prev = string.Empty,
            Self = string.Format("https://localhost:8080/Valeting/flexibilities?pageNumber={0}&pageSize={1}", 1, 2)
        };
        _urlServiceMock.Setup(x => x.GeneratePaginatedLinks(It.IsAny<GeneratePaginatedLinksSVRequest>())).Returns(paginatedLinks);

        var paginationLinksApi = new PaginationLinksApi
        {
            Next = new() { Href = string.Format("https://localhost:8080/Valeting/flexibilities?pageNumber={0}&pageSize={1}", 1, 2) },
            Prev = new() { Href = string.Empty },
            Self = new() { Href = string.Format("https://localhost:8080/Valeting/flexibilities?pageNumber={0}&pageSize={1}", 1, 2) }
        };
        _mapperMock.Setup(x => x.Map<PaginationLinksApi>(paginatedLinks)).Returns(paginationLinksApi);

        var flexibilityApi_List = flexibilitiesSV_List.Select(vs => new FlexibilityApi
        {
            Id = vs.Id,
            Description = vs.Description,
            Active = vs.Active,
            Link = new()
            {
                Self = new()
                {
                    Href = string.Format("https://localhost:8080/Valeting/flexibilities/{0}", vs.Id)
                }
            }
        }).ToList();
        _mapperMock.Setup(x => x.Map<List<FlexibilityApi>>(flexibilitiesSV_List)).Returns(flexibilityApi_List);

        flexibilitiesSV_List.ForEach(x =>
        {
            var generateSelfUrlSVResponse = new GenerateSelfUrlSVResponse() { Self = string.Format("https://localhost:8080/Valeting/flexbilities/{0}", x.Id) };
            _urlServiceMock.Setup(x => x.GenerateSelf(It.IsAny<GenerateSelfUrlSVRequest>())).Returns(generateSelfUrlSVResponse);
        });

        // Act
        var flexibilityController = new FlexibilityController(_flexibilityServiceMock.Object, _urlServiceMock.Object, _cacheHandlerMock.Object, _mapperMock.Object)
        {
            ControllerContext = controllerContext
        };

        var response = await flexibilityController.GetAsync(flexibilityApiParameters);

        // Assert
        Assert.NotNull(response);

        var okResult = response as ObjectResult;
        Assert.NotNull(okResult);
        Assert.Equal(200, okResult.StatusCode);

        var flexibilityApiResponse = okResult.Value as FlexibilityApiPaginatedResponse;
        Assert.NotNull(flexibilityApiResponse);
        Assert.NotEmpty(flexibilityApiResponse.Flexibilities);
        Assert.Equal(3, flexibilityApiResponse.Flexibilities.Count);
        Assert.Equal(1, flexibilityApiResponse.TotalPages);
        Assert.Equal(3, flexibilityApiResponse.TotalItems);
        Assert.NotNull(flexibilityApiResponse.Links);
        Assert.Contains(string.Format("/Valeting/flexibilities?pageNumber={0}&pageSize={1}", 1, 2), flexibilityApiResponse.Links.Next.Href);
        Assert.Contains(string.Format("/Valeting/flexibilities?pageNumber={0}&pageSize={1}", 1, 2), flexibilityApiResponse.Links.Self.Href);
        Assert.Contains(string.Empty, flexibilityApiResponse.Links.Prev.Href);
    }

    [Fact]
    public async Task Get_Status200_WithoutCache_WithError()
    {
        // Arrange
        _httpRequest.Setup(x => x.Host).Returns(HostString.FromUriComponent("http://localhost:8080"));
        _httpRequest.Setup(x => x.Path).Returns(PathString.FromUriComponent("/Valeting/flexibilities"));

        var httpContext = Mock.Of<HttpContext>(x => x.Request == _httpRequest.Object);
        var controllerContext = new ControllerContext()
        {
            HttpContext = httpContext
        };

        var flexibilityApiParameters = new FlexibilityApiParameters
        {
            PageNumber = 1,
            PageSize = 2,
            Active = true
        };

        var paginatedFlexibilitySVResponse = new PaginatedFlexibilitySVResponse
        {
            Error = new()
            {
                ErrorCode = 404, 
                Message = "NotFound" 
            }
        };
        _flexibilityServiceMock.Setup(x => x.GetAsync(It.IsAny<PaginatedFlexibilitySVRequest>())).ReturnsAsync(paginatedFlexibilitySVResponse);

        // Act
        var flexibilityController = new FlexibilityController(_flexibilityServiceMock.Object, _urlServiceMock.Object, _cacheHandlerMock.Object, _mapperMock.Object)
        {
            ControllerContext = controllerContext
        };

        var response = await flexibilityController.GetAsync(flexibilityApiParameters);

        // Assert
        Assert.NotNull(response);

        var notFoundResult = response as ObjectResult;
        Assert.NotNull(notFoundResult);
        Assert.Equal(404, notFoundResult.StatusCode);

        var flexibilityApiResponse = notFoundResult.Value as FlexibilityApiError;
        Assert.NotNull(flexibilityApiResponse);
        Assert.Equal("NotFound", flexibilityApiResponse.Detail);
    }

    [Fact]
    public async Task Get_Status500_WithException()
    {
        // Act
        var flexibilityController = new FlexibilityController(_flexibilityServiceMock.Object, _urlServiceMock.Object, _cacheHandlerMock.Object, _mapperMock.Object);
        var response = await flexibilityController.GetAsync(null);

        // Assert
        Assert.NotNull(response);

        var internalServerErrorResult = response as ObjectResult;
        Assert.NotNull(internalServerErrorResult);
        Assert.Equal(500, internalServerErrorResult.StatusCode);

        var flexibilityApiResponse = internalServerErrorResult.Value as FlexibilityApiError;
        Assert.NotNull(flexibilityApiResponse);
        Assert.False(string.IsNullOrEmpty(flexibilityApiResponse.Detail));
    }
}