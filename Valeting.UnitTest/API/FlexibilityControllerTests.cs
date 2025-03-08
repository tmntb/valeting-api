using AutoMapper;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Valeting.API.Controllers;
using Valeting.Core.Interfaces;
using Valeting.Common.Models.Link;
using Valeting.Common.Models.Flexibility;
using Valeting.Common.Cache.Interfaces;
using Valeting.API.Models.Core;
using Valeting.API.Models.Flexibility;

namespace Valeting.Tests.Api;

public class FlexibilityControllerTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<HttpRequest> _mockHttpRequest;
    private readonly Mock<IUrlService> _mockUrlService;
    private readonly Mock<IFlexibilityService> _mockFlexibilityService;

    public FlexibilityControllerTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockHttpRequest = new Mock<HttpRequest>();
        _mockUrlService = new Mock<IUrlService>();
        _mockFlexibilityService = new Mock<IFlexibilityService>();
    }

    [Fact]
    public async Task GetById_Status200_WithoutCache()
    {
        _mockHttpRequest.Setup(x => x.Host).Returns(HostString.FromUriComponent("http://localhost:8080"));
        _mockHttpRequest.Setup(x => x.Path).Returns(PathString.FromUriComponent("/flexibilities/{0}"));

        var httpContext = Mock.Of<HttpContext>(x => x.Request == _mockHttpRequest.Object);
        var controllerContext = new ControllerContext()
        {
            HttpContext = httpContext
        };

        var id = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var getFlexibilityDtoResponse = new GetFlexibilityDtoResponse
        {
            Flexibility = new()
            {
                Id = id, 
                Description = "flex",
                Active = true
            }
        };
        _mockFlexibilityService.Setup(x => x.GetByIdAsync(It.IsAny<GetFlexibilityDtoRequest>())).ReturnsAsync(getFlexibilityDtoResponse);

        var flexibilityApi = new FlexibilityApi
        {
            Id = id,
            Description = "flex",
            Active = true
        };
        _mockMapper.Setup(x => x.Map<FlexibilityApi>(It.IsAny<FlexibilityDto>())).Returns(flexibilityApi);

        var generateSelfUrlDtoResponse = new GenerateSelfUrlDtoResponse() { Self = string.Format("https://localhost:8080/Valeting/flexibilities/{0}", id) };
        _mockUrlService.Setup(x => x.GenerateSelf(It.IsAny<GenerateSelfUrlDtoRequest>())).Returns(generateSelfUrlDtoResponse);

        // Act
        var flexibilityController = new FlexibilityController(_mockFlexibilityService.Object, _mockUrlService.Object, _mockMapper.Object)
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
        _mockHttpRequest.Setup(x => x.Host).Returns(HostString.FromUriComponent("http://localhost:8080"));
        _mockHttpRequest.Setup(x => x.Path).Returns(PathString.FromUriComponent("/flexibilities/{0}"));

        var httpContext = Mock.Of<HttpContext>(x => x.Request == _mockHttpRequest.Object);
        var controllerContext = new ControllerContext()
        {
            HttpContext = httpContext
        };

        var id = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var getFlexibilityDtoResponse = new GetFlexibilityDtoResponse
        {
            Error = new() 
            {
                ErrorCode = 404, 
                Message = "NotFound" 
            }
        };
        _mockFlexibilityService.Setup(x => x.GetByIdAsync(It.IsAny<GetFlexibilityDtoRequest>())).ReturnsAsync(getFlexibilityDtoResponse);

        // Act
        var flexibilityController = new FlexibilityController(_mockFlexibilityService.Object, _mockUrlService.Object, _mockMapper.Object)
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
        _mockHttpRequest.Setup(x => x.Host).Returns(HostString.FromUriComponent("http://localhost:8080"));
        _mockHttpRequest.Setup(x => x.Path).Returns(PathString.FromUriComponent("/flexibilities/{0}"));

        var httpContext = Mock.Of<HttpContext>(x => x.Request == _mockHttpRequest.Object);
        var controllerContext = new ControllerContext()
        {
            HttpContext = httpContext
        };

        var id = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var getFlexibilityDtoResponse = new GetFlexibilityDtoResponse
        {
            Flexibility = new()
            {
                Id = id, 
                Description = "flex",
                Active = true
            }
        };

        var flexibilityApi = new FlexibilityApi
        {
            Id = id,
            Description = "flex",
            Active = true
        };
        _mockMapper.Setup(x => x.Map<FlexibilityApi>(It.IsAny<FlexibilityDto>())).Returns(flexibilityApi);

        var generateSelfUrlDtoResponse = new GenerateSelfUrlDtoResponse() { Self = string.Format("https://localhost:8080/Valeting/flexibilities/{0}", id) };
        _mockUrlService.Setup(x => x.GenerateSelf(It.IsAny<GenerateSelfUrlDtoRequest>())).Returns(generateSelfUrlDtoResponse);

        // Act
        var flexibilityController = new FlexibilityController(_mockFlexibilityService.Object, _mockUrlService.Object, _mockMapper.Object)
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
        var flexibilityController = new FlexibilityController(_mockFlexibilityService.Object, _mockUrlService.Object, _mockMapper.Object);
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
        _mockHttpRequest.Setup(x => x.Host).Returns(HostString.FromUriComponent("http://localhost:8080"));
        _mockHttpRequest.Setup(x => x.Path).Returns(PathString.FromUriComponent("/Valeting/flexibilities"));

        var httpContext = Mock.Of<HttpContext>(x => x.Request == _mockHttpRequest.Object);
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

        var paginatedFlexibilityDtoRequest = new PaginatedFlexibilityDtoRequest
        {
            Filter = new()
            {
                PageNumber = 1,
                PageSize = 2,
                Active = true
            }
        };
        _mockMapper.Setup(x => x.Map<PaginatedFlexibilityDtoRequest>(flexibilityApiParameters)).Returns(paginatedFlexibilityDtoRequest);

        var flexibilitiesDto_List = new List<FlexibilityDto>()
        {
            new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), Description = "flex1", Active = true },
            new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000003"), Description = "flex2", Active = true },
            new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000004"), Description = "flex3", Active = true }
        };

        var paginatedFlexibilityDtoResponse = new PaginatedFlexibilityDtoResponse
        {
            Flexibilities = flexibilitiesDto_List,
            TotalItems = 3,
            TotalPages = 1
        };
        _mockFlexibilityService.Setup(x => x.GetAsync(It.IsAny<PaginatedFlexibilityDtoRequest>())).ReturnsAsync(paginatedFlexibilityDtoResponse);

        var paginatedLinks = new GeneratePaginatedLinksDtoResponse
        {
            Next = string.Format("https://localhost:8080/Valeting/flexibilities?pageNumber={0}&pageSize={1}", 1, 2),
            Prev = string.Empty,
            Self = string.Format("https://localhost:8080/Valeting/flexibilities?pageNumber={0}&pageSize={1}", 1, 2)
        };
        _mockUrlService.Setup(x => x.GeneratePaginatedLinks(It.IsAny<GeneratePaginatedLinksDtoRequest>())).Returns(paginatedLinks);

        var paginationLinksApi = new PaginationLinksApi
        {
            Next = new() { Href = string.Format("https://localhost:8080/Valeting/flexibilities?pageNumber={0}&pageSize={1}", 1, 2) },
            Prev = new() { Href = string.Empty },
            Self = new() { Href = string.Format("https://localhost:8080/Valeting/flexibilities?pageNumber={0}&pageSize={1}", 1, 2) }
        };
        _mockMapper.Setup(x => x.Map<PaginationLinksApi>(paginatedLinks)).Returns(paginationLinksApi);

        var flexibilityApi_List = flexibilitiesDto_List.Select(vs => new FlexibilityApi
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
        _mockMapper.Setup(x => x.Map<List<FlexibilityApi>>(flexibilitiesDto_List)).Returns(flexibilityApi_List);

        flexibilitiesDto_List.ForEach(x =>
        {
            var generateSelfUrlDtoResponse = new GenerateSelfUrlDtoResponse() { Self = string.Format("https://localhost:8080/Valeting/flexbilities/{0}", x.Id) };
            _mockUrlService.Setup(x => x.GenerateSelf(It.IsAny<GenerateSelfUrlDtoRequest>())).Returns(generateSelfUrlDtoResponse);
        });

        // Act
        var flexibilityController = new FlexibilityController(_mockFlexibilityService.Object, _mockUrlService.Object, _mockMapper.Object)
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
        _mockHttpRequest.Setup(x => x.Host).Returns(HostString.FromUriComponent("http://localhost:8080"));
        _mockHttpRequest.Setup(x => x.Path).Returns(PathString.FromUriComponent("/Valeting/flexibilities"));

        var httpContext = Mock.Of<HttpContext>(x => x.Request == _mockHttpRequest.Object);
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

        var paginatedFlexibilityDtoRequest = new PaginatedFlexibilityDtoRequest
        {
            Filter = new()
            {
                PageNumber = 1,
                PageSize = 2,
                Active = true
            }
        };
        _mockMapper.Setup(x => x.Map<PaginatedFlexibilityDtoRequest>(flexibilityApiParameters)).Returns(paginatedFlexibilityDtoRequest);

        var flexibilitiesDto_List = new List<FlexibilityDto>()
        {
            new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), Description = "flex1", Active = true },
            new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000003"), Description = "flex2", Active = true },
            new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000004"), Description = "flex3", Active = true }
        };

        var paginatedFlexibilityDtoResponse = new PaginatedFlexibilityDtoResponse
        {
            Flexibilities = flexibilitiesDto_List,
            TotalItems = 3,
            TotalPages = 1
        };

        var paginatedLinks = new GeneratePaginatedLinksDtoResponse
        {
            Next = string.Format("https://localhost:8080/Valeting/flexibilities?pageNumber={0}&pageSize={1}", 1, 2),
            Prev = string.Empty,
            Self = string.Format("https://localhost:8080/Valeting/flexibilities?pageNumber={0}&pageSize={1}", 1, 2)
        };
        _mockUrlService.Setup(x => x.GeneratePaginatedLinks(It.IsAny<GeneratePaginatedLinksDtoRequest>())).Returns(paginatedLinks);

        var paginationLinksApi = new PaginationLinksApi
        {
            Next = new() { Href = string.Format("https://localhost:8080/Valeting/flexibilities?pageNumber={0}&pageSize={1}", 1, 2) },
            Prev = new() { Href = string.Empty },
            Self = new() { Href = string.Format("https://localhost:8080/Valeting/flexibilities?pageNumber={0}&pageSize={1}", 1, 2) }
        };
        _mockMapper.Setup(x => x.Map<PaginationLinksApi>(paginatedLinks)).Returns(paginationLinksApi);

        var flexibilityApi_List = flexibilitiesDto_List.Select(vs => new FlexibilityApi
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
        _mockMapper.Setup(x => x.Map<List<FlexibilityApi>>(flexibilitiesDto_List)).Returns(flexibilityApi_List);

        flexibilitiesDto_List.ForEach(x =>
        {
            var generateSelfUrlDtoResponse = new GenerateSelfUrlDtoResponse() { Self = string.Format("https://localhost:8080/Valeting/flexbilities/{0}", x.Id) };
            _mockUrlService.Setup(x => x.GenerateSelf(It.IsAny<GenerateSelfUrlDtoRequest>())).Returns(generateSelfUrlDtoResponse);
        });

        // Act
        var flexibilityController = new FlexibilityController(_mockFlexibilityService.Object, _mockUrlService.Object, _mockMapper.Object)
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
        _mockHttpRequest.Setup(x => x.Host).Returns(HostString.FromUriComponent("http://localhost:8080"));
        _mockHttpRequest.Setup(x => x.Path).Returns(PathString.FromUriComponent("/Valeting/flexibilities"));

        var httpContext = Mock.Of<HttpContext>(x => x.Request == _mockHttpRequest.Object);
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

        var paginatedFlexibilityDtoResponse = new PaginatedFlexibilityDtoResponse
        {
            Error = new()
            {
                ErrorCode = 404, 
                Message = "NotFound" 
            }
        };
        _mockFlexibilityService.Setup(x => x.GetAsync(It.IsAny<PaginatedFlexibilityDtoRequest>())).ReturnsAsync(paginatedFlexibilityDtoResponse);

        // Act
        var flexibilityController = new FlexibilityController(_mockFlexibilityService.Object, _mockUrlService.Object, _mockMapper.Object)
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
        var flexibilityController = new FlexibilityController(_mockFlexibilityService.Object, _mockUrlService.Object, _mockMapper.Object);
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