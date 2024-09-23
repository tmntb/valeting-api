using AutoMapper;

using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using Valeting.Models.Core;
using Valeting.Controllers;
using Valeting.Cache.Interfaces;
using Valeting.Core.Models.Link;
using Valeting.Models.VehicleSize;
using Valeting.Core.Models.VehicleSize;
using Valeting.Core.Services.Interfaces;

namespace Valeting.UnitTest.API;

public class VehicleSizeControllerTests
{
    private Mock<IMapper> _mapperMock;
    private Mock<HttpRequest> _httpRequest;
    private Mock<IUrlService> _urlServiceMock;
    private Mock<ICacheHandler> _cacheHandlerMock;
    private Mock<IVehicleSizeService> _vehicleSizeServiceMock;

    public VehicleSizeControllerTests()
    {
        _mapperMock = new Mock<IMapper>();
        _httpRequest = new Mock<HttpRequest>();
        _urlServiceMock = new Mock<IUrlService>();
        _cacheHandlerMock = new Mock<ICacheHandler>();
        _vehicleSizeServiceMock = new Mock<IVehicleSizeService>();
    }

    [Fact]
    public async Task GetById_Status200_WithoutCache()
    {
        // Arrange
        _httpRequest.Setup(x => x.Host).Returns(HostString.FromUriComponent("http://localhost:8080"));
        _httpRequest.Setup(x => x.Path).Returns(PathString.FromUriComponent("/vehicleSizes/{0}"));

        var httpContext = Mock.Of<HttpContext>(x => x.Request == _httpRequest.Object);
        var controllerContext = new ControllerContext()
        {
            HttpContext = httpContext
        };

        var id = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var getVehicleSizeSVResponse = new GetVehicleSizeSVResponse
        {
            VehicleSize = new()
            {
                Id = id,
                Description = "Van",
                Active = true
            }
        };
        _vehicleSizeServiceMock.Setup(x => x.GetByIdAsync(It.IsAny<GetVehicleSizeSVRequest>())).ReturnsAsync(getVehicleSizeSVResponse);

        var vehicleSizeApi = new VehicleSizeApi
        {
            Id = id,
            Description = "Van",
            Active = true
        };
        _mapperMock.Setup(x => x.Map<VehicleSizeApi>(It.IsAny<VehicleSizeSV>())).Returns(vehicleSizeApi);

        var generateSelfUrlSVResponse = new GenerateSelfUrlSVResponse() { Self = string.Format("https://localhost:8080/Valeting/vehicleSizes/{0}", id) };
        _urlServiceMock.Setup(x => x.GenerateSelf(It.IsAny<GenerateSelfUrlSVRequest>())).Returns(generateSelfUrlSVResponse);

        // Act
        var vehicleSizeController = new VehicleSizeController(_vehicleSizeServiceMock.Object, _urlServiceMock.Object, _cacheHandlerMock.Object, _mapperMock.Object)
        {
            ControllerContext = controllerContext
        };

        var response = await vehicleSizeController.GetByIdAsync(id.ToString());

        // Assert
        Assert.NotNull(response);

        var okResult = response as ObjectResult;
        Assert.NotNull(okResult);
        Assert.Equal(200, okResult.StatusCode);

        var vehicleSizeApiResponse = okResult.Value as VehicleSizeApiResponse;
        Assert.NotNull(vehicleSizeApiResponse);
        Assert.Equal(id, vehicleSizeApiResponse.VehicleSize.Id);
        Assert.Equal("Van", vehicleSizeApiResponse.VehicleSize.Description);
        Assert.True(vehicleSizeApiResponse.VehicleSize.Active);
        Assert.NotNull(vehicleSizeApiResponse.VehicleSize.Link.Self);
        Assert.False(string.IsNullOrEmpty(vehicleSizeApiResponse.VehicleSize.Link.Self.Href));
        Assert.Contains(string.Format("/Valeting/vehicleSizes/{0}", id), vehicleSizeApiResponse.VehicleSize.Link.Self.Href);
    }

    [Fact]
    public async Task GetById_Status200_WithoutCache_WithError()
    {
        // Arrange
        _httpRequest.Setup(x => x.Host).Returns(HostString.FromUriComponent("http://localhost:8080"));
        _httpRequest.Setup(x => x.Path).Returns(PathString.FromUriComponent("/vehicleSizes/{0}"));

        var httpContext = Mock.Of<HttpContext>(x => x.Request == _httpRequest.Object);
        var controllerContext = new ControllerContext()
        {
            HttpContext = httpContext
        };

        var id = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var getVehicleSizeSVResponse = new GetVehicleSizeSVResponse()
        {
            Error = new() 
            { 
                ErrorCode = 404, 
                Message = "NotFound" 
            }
        };
        _vehicleSizeServiceMock.Setup(x => x.GetByIdAsync(It.IsAny<GetVehicleSizeSVRequest>())).ReturnsAsync(getVehicleSizeSVResponse);

        // Act
        var vehicleSizeController = new VehicleSizeController(_vehicleSizeServiceMock.Object, _urlServiceMock.Object, _cacheHandlerMock.Object, _mapperMock.Object)
        {
            ControllerContext = controllerContext
        };

        var response = await vehicleSizeController.GetByIdAsync(id.ToString());

        // Assert
        Assert.NotNull(response);

        var okResult = response as ObjectResult;
        Assert.NotNull(okResult);
        Assert.Equal(404, okResult.StatusCode);

        var vehicleSizeApiResponse = okResult.Value as VehicleSizeApiError;
        Assert.NotNull(vehicleSizeApiResponse);
        Assert.Equal("NotFound", vehicleSizeApiResponse.Detail);
    }

    [Fact]
    public async Task GetById_Status200_WithCache()
    {
        // Arrange
        _httpRequest.Setup(x => x.Host).Returns(HostString.FromUriComponent("http://localhost:8080"));
        _httpRequest.Setup(x => x.Path).Returns(PathString.FromUriComponent("/vehicleSizes/{0}"));

        var httpContext = Mock.Of<HttpContext>(x => x.Request == _httpRequest.Object);
        var controllerContext = new ControllerContext()
        {
            HttpContext = httpContext
        };

        var id = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var getVehicleSizeSVResponse = new GetVehicleSizeSVResponse()
        {
            VehicleSize = new()
            {
                Id = id,
                Description = "Van",
                Active = true
            }
        };
        _cacheHandlerMock.Setup(x => x.GetRecord<GetVehicleSizeSVResponse>(It.IsAny<string>())).Returns(getVehicleSizeSVResponse);

        var vehicleSizeApi = new VehicleSizeApi
        {
            Id = id,
            Description = "Van",
            Active = true
        };
        _mapperMock.Setup(x => x.Map<VehicleSizeApi>(It.IsAny<VehicleSizeSV>())).Returns(vehicleSizeApi);

        var generateSelfUrlSVResponse = new GenerateSelfUrlSVResponse() { Self = string.Format("https://localhost:8080/Valeting/vehicleSizes/{0}", id) };
        _urlServiceMock.Setup(x => x.GenerateSelf(It.IsAny<GenerateSelfUrlSVRequest>())).Returns(generateSelfUrlSVResponse);

        // Act
        var vehicleSizeController = new VehicleSizeController(_vehicleSizeServiceMock.Object, _urlServiceMock.Object, _cacheHandlerMock.Object, _mapperMock.Object)
        {
            ControllerContext = controllerContext
        };

        var response = await vehicleSizeController.GetByIdAsync(id.ToString());

        // Assert
        Assert.NotNull(response);

        var okResult = response as ObjectResult;
        Assert.NotNull(okResult);
        Assert.Equal(200, okResult.StatusCode);

        var vehicleSizeApiResponse = okResult.Value as VehicleSizeApiResponse;
        Assert.NotNull(vehicleSizeApiResponse);
        Assert.Equal(id, vehicleSizeApiResponse.VehicleSize.Id);
        Assert.Equal("Van", vehicleSizeApiResponse.VehicleSize.Description);
        Assert.True(vehicleSizeApiResponse.VehicleSize.Active);
        Assert.NotNull(vehicleSizeApiResponse.VehicleSize.Link.Self);
        Assert.False(string.IsNullOrEmpty(vehicleSizeApiResponse.VehicleSize.Link.Self.Href));
        Assert.Contains(string.Format("/Valeting/vehicleSizes/{0}", id), vehicleSizeApiResponse.VehicleSize.Link.Self.Href);
    }

    [Fact]
    public async Task GetById_Status500_WithException()
    {
        // Act
        var vehicleSizeController = new VehicleSizeController(_vehicleSizeServiceMock.Object, _urlServiceMock.Object, _cacheHandlerMock.Object, _mapperMock.Object);
        var response = await vehicleSizeController.GetByIdAsync(null);

        // Assert
        Assert.NotNull(response);

        var okResult = response as ObjectResult;
        Assert.NotNull(okResult);
        Assert.Equal(500, okResult.StatusCode);

        var vehicleSizeApiResponse = okResult.Value as VehicleSizeApiError;
        Assert.NotNull(vehicleSizeApiResponse);
        Assert.False(string.IsNullOrEmpty(vehicleSizeApiResponse.Detail));
    }

    [Fact]
    public async Task Get_Status200_WithoutCache()
    {
        // Arrange
        _httpRequest.Setup(x => x.Host).Returns(HostString.FromUriComponent("http://localhost:8080"));
        _httpRequest.Setup(x => x.Path).Returns(PathString.FromUriComponent("/Valeting/vehicleSizes"));

        var httpContext = Mock.Of<HttpContext>(x => x.Request == _httpRequest.Object);
        var controllerContext = new ControllerContext()
        {
            HttpContext = httpContext
        };

        var vehicleSizeApiParameters = new VehicleSizeApiParameters
        {
            PageNumber = 1,
            PageSize = 2,
            Active = true
        };

        var paginatedVehicleSizeSVRequest = new PaginatedVehicleSizeSVRequest
        {
            Filter = new()
            {
                PageNumber = 1,
                PageSize = 2,
                Active = true
            }
        };
        _mapperMock.Setup(x => x.Map<PaginatedVehicleSizeSVRequest>(vehicleSizeApiParameters)).Returns(paginatedVehicleSizeSVRequest);

        var vehicleSizesSV_List = new List<VehicleSizeSV>()
        {
            new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), Description = "Van", Active = true },
            new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000003"), Description = "Small", Active = true },
            new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000004"), Description = "Medium", Active = true }
        };

        var paginatedVehicleSizeSVResponse = new PaginatedVehicleSizeSVResponse
        {
            VehicleSizes = vehicleSizesSV_List,
            TotalItems = 3,
            TotalPages = 1
        };
        _vehicleSizeServiceMock.Setup(x => x.GetAsync(It.IsAny<PaginatedVehicleSizeSVRequest>())).ReturnsAsync(paginatedVehicleSizeSVResponse);

        var paginatedLinks = new GeneratePaginatedLinksSVResponse
        {
            Next = string.Format("https://localhost:8080/Valeting/vehicleSizes?pageNumber={0}&pageSize={1}", 1, 2),
            Prev = string.Empty,
            Self = string.Format("https://localhost:8080/Valeting/vehicleSizes?pageNumber={0}&pageSize={1}", 1, 2)
        };
        _urlServiceMock.Setup(x => x.GeneratePaginatedLinks(It.IsAny<GeneratePaginatedLinksSVRequest>())).Returns(paginatedLinks);

        var paginationLinksApi = new PaginationLinksApi
        {
            Next = new() { Href = string.Format("https://localhost:8080/Valeting/vehicleSizes?pageNumber={0}&pageSize={1}", 1, 2) },
            Prev = new() { Href = string.Empty },
            Self = new() { Href = string.Format("https://localhost:8080/Valeting/vehicleSizes?pageNumber={0}&pageSize={1}", 1, 2) }
        };
        _mapperMock.Setup(x => x.Map<PaginationLinksApi>(paginatedLinks)).Returns(paginationLinksApi);

        var vehicleSizeApi_List = vehicleSizesSV_List.Select(vs => new VehicleSizeApi
        {
            Id = vs.Id,
            Description = vs.Description,
            Active = vs.Active,
            Link = new()
            {
                Self = new()
                {
                    Href = string.Format("https://localhost:8080/Valeting/vehicleSizes/{0}", vs.Id)
                }
            }
        }).ToList();
        _mapperMock.Setup(x => x.Map<List<VehicleSizeApi>>(vehicleSizesSV_List)).Returns(vehicleSizeApi_List);

        vehicleSizesSV_List.ForEach(x =>
        {
            var generateSelfUrlSVResponse = new GenerateSelfUrlSVResponse() { Self = string.Format("https://localhost:8080/Valeting/vehicleSizes/{0}", x.Id) };
            _urlServiceMock.Setup(x => x.GenerateSelf(It.IsAny<GenerateSelfUrlSVRequest>())).Returns(generateSelfUrlSVResponse);
        });

        // Act
        var vehicleSizeController = new VehicleSizeController(_vehicleSizeServiceMock.Object, _urlServiceMock.Object, _cacheHandlerMock.Object, _mapperMock.Object)
        {
            ControllerContext = controllerContext
        };

        var response = await vehicleSizeController.GetAsync(vehicleSizeApiParameters);

        // Assert
        Assert.NotNull(response);

        var okResult = response as ObjectResult;
        Assert.NotNull(okResult);
        Assert.Equal(200, okResult.StatusCode);

        var vehicleSizeApiResponse = okResult.Value as VehicleSizeApiPaginatedResponse;
        Assert.NotNull(vehicleSizeApiResponse);
        Assert.NotEmpty(vehicleSizeApiResponse.VehicleSizes);
        Assert.Equal(3, vehicleSizeApiResponse.VehicleSizes.Count);
        Assert.Equal(1, vehicleSizeApiResponse.TotalPages);
        Assert.Equal(3, vehicleSizeApiResponse.TotalItems);
        Assert.NotNull(vehicleSizeApiResponse.Links);
        Assert.Contains(string.Format("/Valeting/vehicleSizes?pageNumber={0}&pageSize={1}", 1, 2), vehicleSizeApiResponse.Links.Next.Href);
        Assert.Contains(string.Format("/Valeting/vehicleSizes?pageNumber={0}&pageSize={1}", 1, 2), vehicleSizeApiResponse.Links.Self.Href);
        Assert.Contains(string.Empty, vehicleSizeApiResponse.Links.Prev.Href);
    }

    [Fact]
    public async Task Get_Status200_WithCache()
    {
        //Arrange
        _httpRequest.Setup(x => x.Host).Returns(HostString.FromUriComponent("http://localhost:8080"));
        _httpRequest.Setup(x => x.Path).Returns(PathString.FromUriComponent("/Valeting/vehicleSizes"));

        var httpContext = Mock.Of<HttpContext>(x => x.Request == _httpRequest.Object);
        var controllerContext = new ControllerContext()
        {
            HttpContext = httpContext
        };

        var vehicleSizeApiParameters = new VehicleSizeApiParameters
        {
            PageNumber = 1,
            PageSize = 2,
            Active = true
        };

        var paginatedVehicleSizeSVRequest = new PaginatedVehicleSizeSVRequest
        {
            Filter = new()
            {
                PageNumber = 1,
                PageSize = 2,
                Active = true
            }
        };
        _mapperMock.Setup(x => x.Map<PaginatedVehicleSizeSVRequest>(vehicleSizeApiParameters)).Returns(paginatedVehicleSizeSVRequest);

        var vehicleSizesSV_List = new List<VehicleSizeSV>()
        {
            new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), Description = "Van", Active = true },
            new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000003"), Description = "Small", Active = true },
            new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000004"), Description = "Medium", Active = true }
        };

        var paginatedVehicleSizeSVResponse = new PaginatedVehicleSizeSVResponse
        {
            VehicleSizes = vehicleSizesSV_List,
            TotalItems = 3,
            TotalPages = 1
        };
        _cacheHandlerMock.Setup(x => x.GetRecord<PaginatedVehicleSizeSVResponse>(It.IsAny<string>())).Returns(paginatedVehicleSizeSVResponse);

        var paginatedLinks = new GeneratePaginatedLinksSVResponse()
        {
            Next = string.Format("https://localhost:8080/Valeting/vehicleSizes?pageNumber={0}&pageSize={1}", 1, 2),
            Prev = string.Empty,
            Self = string.Format("https://localhost:8080/Valeting/vehicleSizes?pageNumber={0}&pageSize={1}", 1, 2),
        };
        _urlServiceMock.Setup(x => x.GeneratePaginatedLinks(It.IsAny<GeneratePaginatedLinksSVRequest>())).Returns(paginatedLinks);

        var paginationLinksApi = new PaginationLinksApi
        {
            Next = new() { Href = string.Format("https://localhost:8080/Valeting/vehicleSizes?pageNumber={0}&pageSize={1}", 1, 2) },
            Prev = new() { Href = string.Empty },
            Self = new() { Href = string.Format("https://localhost:8080/Valeting/vehicleSizes?pageNumber={0}&pageSize={1}", 1, 2) }
        };
        _mapperMock.Setup(x => x.Map<PaginationLinksApi>(paginatedLinks)).Returns(paginationLinksApi);

        var vehicleSizeApi_List = vehicleSizesSV_List.Select(vs => new VehicleSizeApi
        {
            Id = vs.Id,
            Description = vs.Description,
            Active = vs.Active,
            Link = new()
            {
                Self = new()
                {
                    Href = string.Format("https://localhost:8080/Valeting/vehicleSizes/{0}", vs.Id)
                }
            }
        }).ToList();
        _mapperMock.Setup(x => x.Map<List<VehicleSizeApi>>(vehicleSizesSV_List)).Returns(vehicleSizeApi_List);

        vehicleSizesSV_List.ForEach(x =>
        {
            var generateSelfUrlSVResponse = new GenerateSelfUrlSVResponse() { Self = string.Format("https://localhost:8080/Valeting/vehicleSizes/{0}", x.Id) };
            _urlServiceMock.Setup(x => x.GenerateSelf(It.IsAny<GenerateSelfUrlSVRequest>())).Returns(generateSelfUrlSVResponse);
        });

        //Act
        var vehicleSizeController = new VehicleSizeController(_vehicleSizeServiceMock.Object, _urlServiceMock.Object, _cacheHandlerMock.Object, _mapperMock.Object)
        {
            ControllerContext = controllerContext
        };

        var response = await vehicleSizeController.GetAsync(vehicleSizeApiParameters);

        //Assert
        Assert.NotNull(response);

        var okResult = response as ObjectResult;
        Assert.NotNull(okResult);
        Assert.Equal(200, okResult.StatusCode);

        var vehicleSizeApiResponse = okResult.Value as VehicleSizeApiPaginatedResponse;
        Assert.NotNull(vehicleSizeApiResponse);
        Assert.NotEmpty(vehicleSizeApiResponse.VehicleSizes);
        Assert.Equal(3, vehicleSizeApiResponse.VehicleSizes.Count);
        Assert.Equal(1, vehicleSizeApiResponse.TotalPages);
        Assert.Equal(3, vehicleSizeApiResponse.TotalItems);
        Assert.NotNull(vehicleSizeApiResponse.Links);
        Assert.Contains(string.Format("/Valeting/vehicleSizes?pageNumber={0}&pageSize={1}", 1, 2), vehicleSizeApiResponse.Links.Next.Href);
        Assert.Contains(string.Format("/Valeting/vehicleSizes?pageNumber={0}&pageSize={1}", 1, 2), vehicleSizeApiResponse.Links.Self.Href);
        Assert.Contains(string.Empty, vehicleSizeApiResponse.Links.Prev.Href);
    }

    [Fact]
    public async Task Get_Status200_WithoutCache_WithError()
    {
        //Arrange
        _httpRequest.Setup(x => x.Host).Returns(HostString.FromUriComponent("http://localhost:8080"));
        _httpRequest.Setup(x => x.Path).Returns(PathString.FromUriComponent("/Valeting/vehicleSizes"));

        var httpContext = Mock.Of<HttpContext>(x => x.Request == _httpRequest.Object);
        var controllerContext = new ControllerContext()
        {
            HttpContext = httpContext
        };

        var vehicleSizeApiParameters = new VehicleSizeApiParameters
        {
            PageNumber = 1,
            PageSize = 2,
            Active = true
        };

        var paginatedVehicleSizeSVResponse = Task<PaginatedVehicleSizeSVResponse>.Factory.StartNew(() =>
        {
            return new()
            {
                Error = new() { ErrorCode = 404, Message = "NotFound" }
            };
        });
        _vehicleSizeServiceMock.Setup(x => x.GetAsync(It.IsAny<PaginatedVehicleSizeSVRequest>())).Returns(paginatedVehicleSizeSVResponse);

        //Act
        var vehicleSizeController = new VehicleSizeController(_vehicleSizeServiceMock.Object, _urlServiceMock.Object, _cacheHandlerMock.Object, _mapperMock.Object)
        {
            ControllerContext = controllerContext
        };

        var response = await vehicleSizeController.GetAsync(vehicleSizeApiParameters);

        //Assert
        Assert.NotNull(response);

        var notFoundResult = response as ObjectResult;
        Assert.NotNull(notFoundResult);
        Assert.Equal(404, notFoundResult.StatusCode);

        var vehicleSizeApiResponse = notFoundResult.Value as VehicleSizeApiError;
        Assert.NotNull(vehicleSizeApiResponse);
        Assert.Equal("NotFound", vehicleSizeApiResponse.Detail);
    }

    [Fact]
    public async Task Get_Status500_WithException()
    {
        //Act
        var vehicleSizeController = new VehicleSizeController(_vehicleSizeServiceMock.Object, _urlServiceMock.Object, _cacheHandlerMock.Object, _mapperMock.Object);
        var response = await vehicleSizeController.GetAsync(null);

        //Assert
        Assert.NotNull(response);

        var internalServerErrorResult = response as ObjectResult;
        Assert.NotNull(internalServerErrorResult);
        Assert.Equal(500, internalServerErrorResult.StatusCode);

        var vehicleSizeApiResponse = internalServerErrorResult.Value as VehicleSizeApiError;
        Assert.NotNull(vehicleSizeApiResponse);
        Assert.False(string.IsNullOrEmpty(vehicleSizeApiResponse.Detail));
    }
}
