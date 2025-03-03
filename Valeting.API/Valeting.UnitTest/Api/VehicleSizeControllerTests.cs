using AutoMapper;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Valeting.Models.Core;
using Valeting.Controllers;
using Valeting.Core.Interfaces;
using Valeting.Cache.Interfaces;
using Valeting.Models.VehicleSize;
using Valeting.Common.Models.Link;
using Valeting.Common.Models.VehicleSize;

namespace Valeting.Tests.Api;

public class VehicleSizeControllerTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<HttpRequest> _mockHttpRequest;
    private readonly Mock<IUrlService> _mockUrlService;
    private readonly Mock<ICacheHandler> _mockCacheHandler;
    private readonly Mock<IVehicleSizeService> _mockVehicleSizeService;

    public VehicleSizeControllerTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockHttpRequest = new Mock<HttpRequest>();
        _mockUrlService = new Mock<IUrlService>();
        _mockCacheHandler = new Mock<ICacheHandler>();
        _mockVehicleSizeService = new Mock<IVehicleSizeService>();
    }

    [Fact]
    public async Task GetById_Status200_WithoutCache()
    {
        // Arrange
        _mockHttpRequest.Setup(x => x.Host).Returns(HostString.FromUriComponent("http://localhost:8080"));
        _mockHttpRequest.Setup(x => x.Path).Returns(PathString.FromUriComponent("/vehicleSizes/{0}"));

        var httpContext = Mock.Of<HttpContext>(x => x.Request == _mockHttpRequest.Object);
        var controllerContext = new ControllerContext()
        {
            HttpContext = httpContext
        };

        var id = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var getVehicleSizeDtoResponse = new GetVehicleSizeDtoResponse
        {
            VehicleSize = new()
            {
                Id = id,
                Description = "Van",
                Active = true
            }
        };
        _mockVehicleSizeService.Setup(x => x.GetByIdAsync(It.IsAny<GetVehicleSizeDtoRequest>())).ReturnsAsync(getVehicleSizeDtoResponse);

        var vehicleSizeApi = new VehicleSizeApi
        {
            Id = id,
            Description = "Van",
            Active = true
        };
        _mockMapper.Setup(x => x.Map<VehicleSizeApi>(It.IsAny<VehicleSizeDto>())).Returns(vehicleSizeApi);

        var generateSelfUrlDtoResponse = new GenerateSelfUrlDtoResponse() { Self = string.Format("https://localhost:8080/Valeting/vehicleSizes/{0}", id) };
        _mockUrlService.Setup(x => x.GenerateSelf(It.IsAny<GenerateSelfUrlDtoRequest>())).Returns(generateSelfUrlDtoResponse);

        // Act
        var vehicleSizeController = new VehicleSizeController(_mockVehicleSizeService.Object, _mockUrlService.Object, _mockCacheHandler.Object, _mockMapper.Object)
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
        _mockHttpRequest.Setup(x => x.Host).Returns(HostString.FromUriComponent("http://localhost:8080"));
        _mockHttpRequest.Setup(x => x.Path).Returns(PathString.FromUriComponent("/vehicleSizes/{0}"));

        var httpContext = Mock.Of<HttpContext>(x => x.Request == _mockHttpRequest.Object);
        var controllerContext = new ControllerContext()
        {
            HttpContext = httpContext
        };

        var id = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var getVehicleSizeDtoResponse = new GetVehicleSizeDtoResponse()
        {
            Error = new() 
            { 
                ErrorCode = 404, 
                Message = "NotFound" 
            }
        };
        _mockVehicleSizeService.Setup(x => x.GetByIdAsync(It.IsAny<GetVehicleSizeDtoRequest>())).ReturnsAsync(getVehicleSizeDtoResponse);

        // Act
        var vehicleSizeController = new VehicleSizeController(_mockVehicleSizeService.Object, _mockUrlService.Object, _mockCacheHandler.Object, _mockMapper.Object)
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
        _mockHttpRequest.Setup(x => x.Host).Returns(HostString.FromUriComponent("http://localhost:8080"));
        _mockHttpRequest.Setup(x => x.Path).Returns(PathString.FromUriComponent("/vehicleSizes/{0}"));

        var httpContext = Mock.Of<HttpContext>(x => x.Request == _mockHttpRequest.Object);
        var controllerContext = new ControllerContext()
        {
            HttpContext = httpContext
        };

        var id = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var getVehicleSizeDtoResponse = new GetVehicleSizeDtoResponse()
        {
            VehicleSize = new()
            {
                Id = id,
                Description = "Van",
                Active = true
            }
        };
        _mockCacheHandler.Setup(x => x.GetRecord<GetVehicleSizeDtoResponse>(It.IsAny<string>())).Returns(getVehicleSizeDtoResponse);

        var vehicleSizeApi = new VehicleSizeApi
        {
            Id = id,
            Description = "Van",
            Active = true
        };
        _mockMapper.Setup(x => x.Map<VehicleSizeApi>(It.IsAny<VehicleSizeDto>())).Returns(vehicleSizeApi);

        var generateSelfUrlDtoResponse = new GenerateSelfUrlDtoResponse() { Self = string.Format("https://localhost:8080/Valeting/vehicleSizes/{0}", id) };
        _mockUrlService.Setup(x => x.GenerateSelf(It.IsAny<GenerateSelfUrlDtoRequest>())).Returns(generateSelfUrlDtoResponse);

        // Act
        var vehicleSizeController = new VehicleSizeController(_mockVehicleSizeService.Object, _mockUrlService.Object, _mockCacheHandler.Object, _mockMapper.Object)
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
        var vehicleSizeController = new VehicleSizeController(_mockVehicleSizeService.Object, _mockUrlService.Object, _mockCacheHandler.Object, _mockMapper.Object);
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
        _mockHttpRequest.Setup(x => x.Host).Returns(HostString.FromUriComponent("http://localhost:8080"));
        _mockHttpRequest.Setup(x => x.Path).Returns(PathString.FromUriComponent("/Valeting/vehicleSizes"));

        var httpContext = Mock.Of<HttpContext>(x => x.Request == _mockHttpRequest.Object);
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

        var paginatedVehicleSizeDtoRequest = new PaginatedVehicleSizeDtoRequest
        {
            Filter = new()
            {
                PageNumber = 1,
                PageSize = 2,
                Active = true
            }
        };
        _mockMapper.Setup(x => x.Map<PaginatedVehicleSizeDtoRequest>(vehicleSizeApiParameters)).Returns(paginatedVehicleSizeDtoRequest);

        var vehicleSizesDto_List = new List<VehicleSizeDto>()
        {
            new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), Description = "Van", Active = true },
            new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000003"), Description = "Small", Active = true },
            new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000004"), Description = "Medium", Active = true }
        };

        var paginatedVehicleSizeDtoResponse = new PaginatedVehicleSizeDtoResponse
        {
            VehicleSizes = vehicleSizesDto_List,
            TotalItems = 3,
            TotalPages = 1
        };
        _mockVehicleSizeService.Setup(x => x.GetAsync(It.IsAny<PaginatedVehicleSizeDtoRequest>())).ReturnsAsync(paginatedVehicleSizeDtoResponse);

        var paginatedLinks = new GeneratePaginatedLinksDtoResponse
        {
            Next = string.Format("https://localhost:8080/Valeting/vehicleSizes?pageNumber={0}&pageSize={1}", 1, 2),
            Prev = string.Empty,
            Self = string.Format("https://localhost:8080/Valeting/vehicleSizes?pageNumber={0}&pageSize={1}", 1, 2)
        };
        _mockUrlService.Setup(x => x.GeneratePaginatedLinks(It.IsAny<GeneratePaginatedLinksDtoRequest>())).Returns(paginatedLinks);

        var paginationLinksApi = new PaginationLinksApi
        {
            Next = new() { Href = string.Format("https://localhost:8080/Valeting/vehicleSizes?pageNumber={0}&pageSize={1}", 1, 2) },
            Prev = new() { Href = string.Empty },
            Self = new() { Href = string.Format("https://localhost:8080/Valeting/vehicleSizes?pageNumber={0}&pageSize={1}", 1, 2) }
        };
        _mockMapper.Setup(x => x.Map<PaginationLinksApi>(paginatedLinks)).Returns(paginationLinksApi);

        var vehicleSizeApi_List = vehicleSizesDto_List.Select(vs => new VehicleSizeApi
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
        _mockMapper.Setup(x => x.Map<List<VehicleSizeApi>>(vehicleSizesDto_List)).Returns(vehicleSizeApi_List);

        vehicleSizesDto_List.ForEach(x =>
        {
            var generateSelfUrlDtoResponse = new GenerateSelfUrlDtoResponse() { Self = string.Format("https://localhost:8080/Valeting/vehicleSizes/{0}", x.Id) };
            _mockUrlService.Setup(x => x.GenerateSelf(It.IsAny<GenerateSelfUrlDtoRequest>())).Returns(generateSelfUrlDtoResponse);
        });

        // Act
        var vehicleSizeController = new VehicleSizeController(_mockVehicleSizeService.Object, _mockUrlService.Object, _mockCacheHandler.Object, _mockMapper.Object)
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
        _mockHttpRequest.Setup(x => x.Host).Returns(HostString.FromUriComponent("http://localhost:8080"));
        _mockHttpRequest.Setup(x => x.Path).Returns(PathString.FromUriComponent("/Valeting/vehicleSizes"));

        var httpContext = Mock.Of<HttpContext>(x => x.Request == _mockHttpRequest.Object);
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

        var paginatedVehicleSizeDtoRequest = new PaginatedVehicleSizeDtoRequest
        {
            Filter = new()
            {
                PageNumber = 1,
                PageSize = 2,
                Active = true
            }
        };
        _mockMapper.Setup(x => x.Map<PaginatedVehicleSizeDtoRequest>(vehicleSizeApiParameters)).Returns(paginatedVehicleSizeDtoRequest);

        var vehicleSizesDto_List = new List<VehicleSizeDto>()
        {
            new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), Description = "Van", Active = true },
            new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000003"), Description = "Small", Active = true },
            new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000004"), Description = "Medium", Active = true }
        };

        var paginatedVehicleSizeDtoResponse = new PaginatedVehicleSizeDtoResponse
        {
            VehicleSizes = vehicleSizesDto_List,
            TotalItems = 3,
            TotalPages = 1
        };
        _mockCacheHandler.Setup(x => x.GetRecord<PaginatedVehicleSizeDtoResponse>(It.IsAny<string>())).Returns(paginatedVehicleSizeDtoResponse);

        var paginatedLinks = new GeneratePaginatedLinksDtoResponse()
        {
            Next = string.Format("https://localhost:8080/Valeting/vehicleSizes?pageNumber={0}&pageSize={1}", 1, 2),
            Prev = string.Empty,
            Self = string.Format("https://localhost:8080/Valeting/vehicleSizes?pageNumber={0}&pageSize={1}", 1, 2),
        };
        _mockUrlService.Setup(x => x.GeneratePaginatedLinks(It.IsAny<GeneratePaginatedLinksDtoRequest>())).Returns(paginatedLinks);

        var paginationLinksApi = new PaginationLinksApi
        {
            Next = new() { Href = string.Format("https://localhost:8080/Valeting/vehicleSizes?pageNumber={0}&pageSize={1}", 1, 2) },
            Prev = new() { Href = string.Empty },
            Self = new() { Href = string.Format("https://localhost:8080/Valeting/vehicleSizes?pageNumber={0}&pageSize={1}", 1, 2) }
        };
        _mockMapper.Setup(x => x.Map<PaginationLinksApi>(paginatedLinks)).Returns(paginationLinksApi);

        var vehicleSizeApi_List = vehicleSizesDto_List.Select(vs => new VehicleSizeApi
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
        _mockMapper.Setup(x => x.Map<List<VehicleSizeApi>>(vehicleSizesDto_List)).Returns(vehicleSizeApi_List);

        vehicleSizesDto_List.ForEach(x =>
        {
            var generateSelfUrlDtoResponse = new GenerateSelfUrlDtoResponse() { Self = string.Format("https://localhost:8080/Valeting/vehicleSizes/{0}", x.Id) };
            _mockUrlService.Setup(x => x.GenerateSelf(It.IsAny<GenerateSelfUrlDtoRequest>())).Returns(generateSelfUrlDtoResponse);
        });

        //Act
        var vehicleSizeController = new VehicleSizeController(_mockVehicleSizeService.Object, _mockUrlService.Object, _mockCacheHandler.Object, _mockMapper.Object)
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
        _mockHttpRequest.Setup(x => x.Host).Returns(HostString.FromUriComponent("http://localhost:8080"));
        _mockHttpRequest.Setup(x => x.Path).Returns(PathString.FromUriComponent("/Valeting/vehicleSizes"));

        var httpContext = Mock.Of<HttpContext>(x => x.Request == _mockHttpRequest.Object);
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

        var paginatedVehicleSizeDtoResponse = Task<PaginatedVehicleSizeDtoResponse>.Factory.StartNew(() =>
        {
            return new()
            {
                Error = new() { ErrorCode = 404, Message = "NotFound" }
            };
        });
        _mockVehicleSizeService.Setup(x => x.GetAsync(It.IsAny<PaginatedVehicleSizeDtoRequest>())).Returns(paginatedVehicleSizeDtoResponse);

        //Act
        var vehicleSizeController = new VehicleSizeController(_mockVehicleSizeService.Object, _mockUrlService.Object, _mockCacheHandler.Object, _mockMapper.Object)
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
        var vehicleSizeController = new VehicleSizeController(_mockVehicleSizeService.Object, _mockUrlService.Object, _mockCacheHandler.Object, _mockMapper.Object);
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
