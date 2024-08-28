using Moq;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using Valeting.Controllers;
using Valeting.Cache.Interfaces;
using Valeting.Core.Services.Interfaces;
using Valeting.Repository.Models.VehicleSize;
using Valeting.Models.VehicleSize;
using Valeting.Core.Models.VehicleSize;
using Valeting.Core.Models.Link;
using Valeting.Models.Core;

namespace Valeting.UnitTest.Controller;

public class VehicleSizeControllerTest
{
    // [Fact]
    // public void FindById_Status200_WithoutCache()
    // {
    //     //Arrange
    //     var redisCacheMock = new Mock<IRedisCache>();
    //     var urlServiceMock = new Mock<IUrlService>();
    //     var vehicleSizeServiceMock = new Mock<IVehicleSizeService>();

    //     var request = new Mock<HttpRequest>();
    //     request.Setup(x => x.Host).Returns(HostString.FromUriComponent("http://localhost:8080"));
    //     request.Setup(x => x.Path).Returns(PathString.FromUriComponent("/vehicleSizes/{0}"));

    //     var httpContext = Mock.Of<HttpContext>(x => x.Request == request.Object);
    //     var controllerContext = new ControllerContext()
    //     {
    //         HttpContext = httpContext
    //     };

    //     var id = Guid.NewGuid();
    //     var getVehicleSizeSVResponse_Mock = Task<GetVehicleSizeSVResponse>.Factory.StartNew(() =>
    //     {
    //         return new GetVehicleSizeSVResponse()
    //         {
    //             Id = id,
    //             Description = "Van",
    //             Active = It.IsAny<bool>()
    //         };
    //     });
    //     vehicleSizeServiceMock.Setup(x => x.GetByIdAsync(It.IsAny<GetVehicleSizeSVRequest>())).Returns(getVehicleSizeSVResponse_Mock);

    //     var href_Mock = new GenerateSelfUrlSVResponse() { Self = string.Format("https://localhost:8080/Valeting/vehicleSizes/{0}", id) };
    //     urlServiceMock.Setup(x => x.GenerateSelf(It.IsAny<GenerateSelfUrlSVRequest>())).Returns(href_Mock);

    //     //Act
    //     var vehicleSizeController = new VehicleSizeController(redisCacheMock.Object, vehicleSizeServiceMock.Object, urlServiceMock.Object)
    //     {
    //         ControllerContext = controllerContext
    //     };

    //     var objResult = (ObjectResult)vehicleSizeController.GetByIdAsync(id.ToString()).ConfigureAwait(false).GetAwaiter().GetResult();
    //     var response = (VehicleSizeApiResponse)objResult.Value;

    //     //Assert
    //     Assert.NotNull(objResult);
    //     Assert.Equal(200, objResult.StatusCode);
    //     Assert.True(objResult.Value.GetType() == typeof(VehicleSizeApiResponse));
    //     Assert.NotNull(response);
    //     Assert.True(response.VehicleSize.Id.Equals(id));
    //     Assert.Equal("Van", response.VehicleSize.Description);
    //     Assert.False(response.VehicleSize.Active);
    //     Assert.NotNull(response.VehicleSize.Link.Self);
    //     Assert.False(string.IsNullOrEmpty(response.VehicleSize.Link.Self.Href));
    //     Assert.Contains(string.Format("/Valeting/vehicleSizes/{0}", id), response.VehicleSize.Link.Self.Href);
    // }

    // [Fact]
    // public void FindById_Status200_WithoutCache_WithError()
    // {
    //     //Arrange
    //     var redisCacheMock = new Mock<IRedisCache>();
    //     var urlServiceMock = new Mock<IUrlService>();
    //     var vehicleSizeServiceMock = new Mock<IVehicleSizeService>();

    //     var request = new Mock<HttpRequest>();
    //     request.Setup(x => x.Host).Returns(HostString.FromUriComponent("http://localhost:8080"));
    //     request.Setup(x => x.Path).Returns(PathString.FromUriComponent("/vehicleSizes/{0}"));

    //     var httpContext = Mock.Of<HttpContext>(x => x.Request == request.Object);
    //     var controllerContext = new ControllerContext()
    //     {
    //         HttpContext = httpContext
    //     };

    //     var id = Guid.NewGuid();
    //     var getVehicleSizeSVResponse_Mock = Task<GetVehicleSizeSVResponse>.Factory.StartNew(() =>
    //     {
    //         return new()
    //         {
    //             Error = new() { ErrorCode = 404, Message = "NotFound" }
    //         };
    //     });
    //     vehicleSizeServiceMock.Setup(x => x.GetByIdAsync(It.IsAny<GetVehicleSizeSVRequest>())).Returns(getVehicleSizeSVResponse_Mock);

    //     //Act
    //     var vehicleSizeController = new VehicleSizeController(redisCacheMock.Object, vehicleSizeServiceMock.Object, urlServiceMock.Object)
    //     {
    //         ControllerContext = controllerContext
    //     };

    //     var objResult = (ObjectResult)vehicleSizeController.GetByIdAsync(id.ToString()).ConfigureAwait(false).GetAwaiter().GetResult();
    //     var response = (VehicleSizeApiError)objResult.Value;

    //     //Assert
    //     Assert.NotNull(objResult);
    //     Assert.Equal(404, objResult.StatusCode);
    //     Assert.True(objResult.Value.GetType() == typeof(VehicleSizeApiError));
    //     Assert.NotNull(response);
    //     Assert.Equal("NotFound", response.Detail);
    // }

    // [Fact]
    // public void FindById_Status200_WithCache()
    // {
    //     //Arrange
    //     var redisCacheMock = new Mock<IRedisCache>();
    //     var urlServiceMock = new Mock<IUrlService>();
    //     var vehicleSizeServiceMock = new Mock<IVehicleSizeService>();

    //     var request = new Mock<HttpRequest>();
    //     request.Setup(x => x.Host).Returns(HostString.FromUriComponent("http://localhost:8080"));
    //     request.Setup(x => x.Path).Returns(PathString.FromUriComponent("/vehicleSizes/{0}"));

    //     var httpContext = Mock.Of<HttpContext>(x => x.Request == request.Object);
    //     var controllerContext = new ControllerContext()
    //     {
    //         HttpContext = httpContext
    //     };

    //     var id = Guid.NewGuid();
    //     var getVehicleSizeSVResponse_Mock = Task<GetVehicleSizeSVResponse>.Factory.StartNew(() =>
    //     {
    //         return new GetVehicleSizeSVResponse()
    //         {
    //             Id = id,
    //             Description = "Van",
    //             Active = It.IsAny<bool>(),
    //         };
    //     });
    //     redisCacheMock.Setup(x => x.GetRecordAsync<GetVehicleSizeSVResponse>(It.IsAny<string>())).Returns(getVehicleSizeSVResponse_Mock);

    //     var href_Mock = new GenerateSelfUrlSVResponse() { Self = string.Format("https://localhost:8080/Valeting/vehicleSizes/{0}", id) };
    //     urlServiceMock.Setup(x => x.GenerateSelf(It.IsAny<GenerateSelfUrlSVRequest>())).Returns(href_Mock);

    //     //Act
    //     var vehicleSizeController = new VehicleSizeController(redisCacheMock.Object, vehicleSizeServiceMock.Object, urlServiceMock.Object)
    //     {
    //         ControllerContext = controllerContext
    //     };

    //     var objResult = (ObjectResult)vehicleSizeController.GetByIdAsync(id.ToString()).ConfigureAwait(false).GetAwaiter().GetResult();
    //     var response = (VehicleSizeApiResponse)objResult.Value;

    //     //Assert
    //     Assert.NotNull(objResult);
    //     Assert.Equal(200, objResult.StatusCode);
    //     Assert.True(objResult.Value.GetType() == typeof(VehicleSizeApiResponse));
    //     Assert.NotNull(response);
    //     Assert.True(response.VehicleSize.Id.Equals(id));
    //     Assert.Equal("Van", response.VehicleSize.Description);
    //     Assert.False(response.VehicleSize.Active);
    //     Assert.NotNull(response.VehicleSize.Link.Self);
    //     Assert.False(string.IsNullOrEmpty(response.VehicleSize.Link.Self.Href));
    //     Assert.Contains(string.Format("/Valeting/vehicleSizes/{0}", id), response.VehicleSize.Link.Self.Href);
    // }

    // [Fact]
    // public void FindById_Status500_WithException()
    // {
    //     //Act
    //     var redisCacheMock = new Mock<IRedisCache>();
    //     var urlServiceMock = new Mock<IUrlService>();
    //     var vehicleSizeServiceMock = new Mock<IVehicleSizeService>();

    //     var vehicleSizeController = new VehicleSizeController(redisCacheMock.Object, vehicleSizeServiceMock.Object, urlServiceMock.Object);
    //     var objResult = (ObjectResult)vehicleSizeController.GetByIdAsync(null).ConfigureAwait(false).GetAwaiter().GetResult();
    //     var response = (VehicleSizeApiError)objResult.Value;

    //     //Assert
    //     Assert.NotNull(objResult);
    //     Assert.Equal(500, objResult.StatusCode);
    //     Assert.NotNull(response);
    //     Assert.False(string.IsNullOrEmpty(response.Detail));
    // }

    // [Fact]
    // public void ListAll_Status200_WithoutCache() 
    // {
    //     //Arrange
    //     var redisCacheMock = new Mock<IRedisCache>();
    //     var urlServiceMock = new Mock<IUrlService>();
    //     var vehicleSizeServiceMock = new Mock<IVehicleSizeService>();

    //     var request = new Mock<HttpRequest>();
    //     request.Setup(x => x.Host).Returns(HostString.FromUriComponent("http://localhost:8080"));
    //     request.Setup(x => x.Path).Returns(PathString.FromUriComponent("/Valeting/vehicleSizes"));

    //     var httpContext = Mock.Of<HttpContext>(x => x.Request == request.Object);
    //     var controllerContext = new ControllerContext()
    //     {
    //         HttpContext = httpContext
    //     };

    //     var vehicleSizeApiParameters_Mock = new Mock<VehicleSizeApiParameters>();
    //     var vehicleSizesSV_List = new List<VehicleSizeSV>()
    //     {
    //         new() { Id = Guid.NewGuid(), Description = "Van", Active = It.IsAny<bool>() },
    //         new() { Id = Guid.NewGuid(), Description = "Small", Active = It.IsAny<bool>() },
    //         new() { Id = Guid.NewGuid(), Description = "Medium", Active = It.IsAny<bool>() }
    //     };

    //     var paginatedVehicleSizeSVResponse_Mock = Task<PaginatedVehicleSizeSVResponse>.Factory.StartNew(() =>
    //     {
    //         return new()
    //         {
    //             VehicleSizes = vehicleSizesSV_List,
    //             TotalItems = 3,
    //             TotalPages = 1
    //         };
    //     });
    //     vehicleSizeServiceMock.Setup(x => x.GetAsync(It.IsAny<PaginatedVehicleSizeSVRequest>())).Returns(paginatedVehicleSizeSVResponse_Mock);

    //     var paginatedLinks_Mock = new GeneratePaginatedLinksSVResponse()
    //     {
    //         Next = string.Format("https://localhost:8080/Valeting/vehicleSizes?pageNumber={0}&pageSize={1}", 1, 2),
    //         Prev = string.Empty,
    //         Self = string.Format("https://localhost:8080/Valeting/vehicleSizes?pageNumber={0}&pageSize={1}", 1, 2),
    //     };
    //     urlServiceMock.Setup(x => x.GeneratePaginatedLinks(It.IsAny<GeneratePaginatedLinksSVRequest>())).Returns(paginatedLinks_Mock);

    //     vehicleSizesSV_List.ForEach(x =>
    //     {
    //         var href_Mock = new GenerateSelfUrlSVResponse() { Self = string.Format("https://localhost:8080/Valeting/vehicleSizes/{0}", x.Id) };
    //         urlServiceMock.Setup(x => x.GenerateSelf(It.IsAny<GenerateSelfUrlSVRequest>())).Returns(href_Mock);
    //     });

    //     //Act
    //     var vehicleSizeController = new VehicleSizeController(redisCacheMock.Object, vehicleSizeServiceMock.Object, urlServiceMock.Object)
    //     {
    //         ControllerContext = controllerContext
    //     };

    //     var objResult = (ObjectResult)vehicleSizeController.GetAsync(vehicleSizeApiParameters_Mock.Object).ConfigureAwait(false).GetAwaiter().GetResult();
    //     var response = (VehicleSizeApiPaginatedResponse)objResult.Value;

    //     //Assert
    //     Assert.NotNull(objResult);
    //     Assert.Equal(200, objResult.StatusCode);
    //     Assert.True(objResult.Value.GetType() == typeof(VehicleSizeApiPaginatedResponse));
    //     Assert.NotNull(response);
    //     Assert.NotEmpty(response.VehicleSizes);
    //     Assert.Equal(3, response.VehicleSizes.Count);
    //     Assert.Equal(1, response.TotalPages);
    //     Assert.Equal(3, response.TotalItems);
    //     Assert.NotNull(response.Links);
    //     Assert.Contains(string.Format("/Valeting/vehicleSizes?pageNumber={0}&pageSize={1}", 1, 2), response.Links.Next.Href);
    //     Assert.Contains(string.Format("/Valeting/vehicleSizes?pageNumber={0}&pageSize={1}", 1, 2), response.Links.Self.Href);
    //     Assert.Contains(string.Empty, response.Links.Prev.Href);
    // }

    // [Fact]
    // public void ListAll_Status200_WithCache()
    // {
    //     //Arrange
    //     var redisCacheMock = new Mock<IRedisCache>();
    //     var urlServiceMock = new Mock<IUrlService>();
    //     var vehicleSizeServiceMock = new Mock<IVehicleSizeService>();

    //     var request = new Mock<HttpRequest>();
    //     request.Setup(x => x.Host).Returns(HostString.FromUriComponent("http://localhost:8080"));
    //     request.Setup(x => x.Path).Returns(PathString.FromUriComponent("/Valeting/vehicleSizes"));

    //     var httpContext = Mock.Of<HttpContext>(x => x.Request == request.Object);
    //     var controllerContext = new ControllerContext()
    //     {
    //         HttpContext = httpContext
    //     };

    //     var vehicleSizeApiParameters_Mock = new Mock<VehicleSizeApiParameters>();
    //     var vehicleSizesSV_List = new List<VehicleSizeSV>()
    //     {
    //         new() { Id = Guid.NewGuid(), Description = "Van", Active = It.IsAny<bool>() },
    //         new() { Id = Guid.NewGuid(), Description = "Small", Active = It.IsAny<bool>() },
    //         new() { Id = Guid.NewGuid(), Description = "Medium", Active = It.IsAny<bool>() }
    //     };

    //     var paginatedVehicleSizeSVResponse_Mock = Task<PaginatedVehicleSizeSVResponse>.Factory.StartNew(() =>
    //     {
    //         return new()
    //         {
    //             VehicleSizes = vehicleSizesSV_List,
    //             TotalItems = 3,
    //             TotalPages = 1
    //         };
    //     });
    //     redisCacheMock.Setup(x => x.GetRecordAsync<PaginatedVehicleSizeSVResponse>(It.IsAny<string>())).Returns(paginatedVehicleSizeSVResponse_Mock);

    //     var paginatedLinks_Mock = new GeneratePaginatedLinksSVResponse()
    //     {
    //         Next = string.Format("https://localhost:8080/Valeting/vehicleSizes?pageNumber={0}&pageSize={1}", 1, 2),
    //         Prev = string.Empty,
    //         Self = string.Format("https://localhost:8080/Valeting/vehicleSizes?pageNumber={0}&pageSize={1}", 1, 2),
    //     };
    //     urlServiceMock.Setup(x => x.GeneratePaginatedLinks(It.IsAny<GeneratePaginatedLinksSVRequest>())).Returns(paginatedLinks_Mock);

    //     vehicleSizesSV_List.ForEach(x =>
    //     {
    //         var href_Mock = new GenerateSelfUrlSVResponse() { Self = string.Format("https://localhost:8080/Valeting/vehicleSizes/{0}", x.Id) };
    //         urlServiceMock.Setup(x => x.GenerateSelf(It.IsAny<GenerateSelfUrlSVRequest>())).Returns(href_Mock);
    //     });

    //     //Act
    //     var vehicleSizeController = new VehicleSizeController(redisCacheMock.Object, vehicleSizeServiceMock.Object, urlServiceMock.Object)
    //     {
    //         ControllerContext = controllerContext
    //     };

    //     var objResult = (ObjectResult)vehicleSizeController.GetAsync(vehicleSizeApiParameters_Mock.Object).ConfigureAwait(false).GetAwaiter().GetResult();
    //     var response = (VehicleSizeApiPaginatedResponse)objResult.Value;

    //     //Assert
    //     Assert.NotNull(objResult);
    //     Assert.Equal(200, objResult.StatusCode);
    //     Assert.True(objResult.Value.GetType() == typeof(VehicleSizeApiPaginatedResponse));
    //     Assert.NotNull(response);
    //     Assert.NotEmpty(response.VehicleSizes);
    //     Assert.Equal(3, response.VehicleSizes.Count);
    //     Assert.Equal(1, response.TotalPages);
    //     Assert.Equal(3, response.TotalItems);
    //     Assert.NotNull(response.Links);
    //     Assert.Contains(string.Format("/Valeting/vehicleSizes?pageNumber={0}&pageSize={1}", 1, 2), response.Links.Next.Href);
    //     Assert.Contains(string.Format("/Valeting/vehicleSizes?pageNumber={0}&pageSize={1}", 1, 2), response.Links.Self.Href);
    //     Assert.Contains(string.Empty, response.Links.Prev.Href);
    // }

    // [Fact]
    // public void ListAll_Status200__WithoutCache_WithError() 
    // {
    //     //Arrange
    //     var redisCacheMock = new Mock<IRedisCache>();
    //     var urlServiceMock = new Mock<IUrlService>();
    //     var vehicleSizeServiceMock = new Mock<IVehicleSizeService>();

    //     var request = new Mock<HttpRequest>();
    //     request.Setup(x => x.Host).Returns(HostString.FromUriComponent("http://localhost:8080"));
    //     request.Setup(x => x.Path).Returns(PathString.FromUriComponent("/Valeting/vehicleSizes"));

    //     var httpContext = Mock.Of<HttpContext>(x => x.Request == request.Object);
    //     var controllerContext = new ControllerContext()
    //     {
    //         HttpContext = httpContext
    //     };

    //     var vehicleSizeApiParameters_Mock = new Mock<VehicleSizeApiParameters>();

    //     var paginatedVehicleSizeSVResponse_Mock = Task<PaginatedVehicleSizeSVResponse>.Factory.StartNew(() =>
    //     {
    //         return new()
    //         {
    //             Error = new() { ErrorCode = 404, Message = "NotFound" }
    //         };
    //     });
    //     vehicleSizeServiceMock.Setup(x => x.GetAsync(It.IsAny<PaginatedVehicleSizeSVRequest>())).Returns(paginatedVehicleSizeSVResponse_Mock);

    //     //Act
    //     var vehicleSizeController = new VehicleSizeController(redisCacheMock.Object, vehicleSizeServiceMock.Object, urlServiceMock.Object)
    //     {
    //         ControllerContext = controllerContext
    //     };

    //     var objResult = (ObjectResult)vehicleSizeController.GetAsync(vehicleSizeApiParameters_Mock.Object).ConfigureAwait(false).GetAwaiter().GetResult();
    //     var response = (VehicleSizeApiError)objResult.Value;

    //     //Assert
    //     Assert.NotNull(objResult);
    //     Assert.Equal(404, objResult.StatusCode);
    //     Assert.True(objResult.Value.GetType() == typeof(VehicleSizeApiError));
    //     Assert.NotNull(response);
    //     Assert.Equal("NotFound", response.Detail);
    // }

    // [Fact]
    // public void ListAll_Status500_WithException()
    // {
    //     //Arrange
    //     var redisCacheMock = new Mock<IRedisCache>();
    //     var urlServiceMock = new Mock<IUrlService>();
    //     var vehicleSizeServiceMock = new Mock<IVehicleSizeService>();

    //     //Act
    //     var vehicleSizeController = new VehicleSizeController(redisCacheMock.Object, vehicleSizeServiceMock.Object, urlServiceMock.Object);
    //     var objResult = (ObjectResult)vehicleSizeController.GetAsync(null).ConfigureAwait(false).GetAwaiter().GetResult();
    //     var response = (VehicleSizeApiError)objResult.Value;

    //     //Assert
    //     Assert.NotNull(objResult);
    //     Assert.Equal(500, objResult.StatusCode);
    //     Assert.NotNull(response);
    //     Assert.False(string.IsNullOrEmpty(response.Detail));
    // }
}
