using Moq;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using Valeting.Controllers;
using Valeting.Business.Core;
using Valeting.Common.Exceptions;
using Valeting.Helpers.Interfaces;
using Valeting.Services.Interfaces;
using Valeting.Business.VehicleSize;
using Valeting.ApiObjects.VehicleSize;

namespace Valeting.UnitTest.Controller;

public class VehicleSizeControllerTest
{ 
    [Fact]
    public void FindById_Status200_WithoutCache()
    {
        //Arrange
        var redisCacheMock = new Mock<IRedisCache>();
        var urlServiceMock = new Mock<IUrlService>();
        var vehicleSizeServiceMock = new Mock<IVehicleSizeService>();

        var request = new Mock<HttpRequest>();
        request.Setup(x => x.Host).Returns(HostString.FromUriComponent("http://localhost:8080"));
        request.Setup(x => x.Path).Returns(PathString.FromUriComponent("/vehicleSizes/{0}"));

        var httpContext = Mock.Of<HttpContext>(x => x.Request == request.Object);
        var controllerContext = new ControllerContext()
        {
            HttpContext = httpContext
        };

        var id = Guid.NewGuid();
        var vehicleSizeDTO_Mock = Task<VehicleSizeDTO>.Factory.StartNew(() =>
        {
            return new VehicleSizeDTO()
            {
                Id = id,
                Description = "Van",
                Active = It.IsAny<bool>()
            };
        });
        vehicleSizeServiceMock.Setup(x => x.FindByIDAsync(id)).Returns(vehicleSizeDTO_Mock);

        var href_Mock = string.Format("https://localhost:8080/Valeting/vehicleSizes/{0}", id);
        urlServiceMock.Setup(x => x.GenerateSelf(It.IsAny<string>(), It.IsAny<string>())).Returns(href_Mock);

        //Act
        var vehicleSizeController = new VehicleSizeController(redisCacheMock.Object, vehicleSizeServiceMock.Object, urlServiceMock.Object)
        {
            ControllerContext = controllerContext
        };

        var objResult = (ObjectResult)vehicleSizeController.FindByIdAsync(id.ToString()).ConfigureAwait(false).GetAwaiter().GetResult();
        var response = (VehicleSizeApiResponse)objResult.Value;

        //Assert
        Assert.NotNull(objResult);
        Assert.Equal(200, objResult.StatusCode);
        Assert.True(objResult.Value.GetType() == typeof(VehicleSizeApiResponse));
        Assert.NotNull(response);
        Assert.True(response.VehicleSize.Id.Equals(id));
        Assert.Equal("Van", response.VehicleSize.Description);
        Assert.False(response.VehicleSize.Active);
        Assert.NotNull(response.VehicleSize.Link.Self);
        Assert.False(string.IsNullOrEmpty(response.VehicleSize.Link.Self.Href));
        Assert.Contains(string.Format("/Valeting/vehicleSizes/{0}", id), response.VehicleSize.Link.Self.Href);
    }

    [Fact]
    public void FindById_Status200_WithCache()
    {
        //Arrange
        var redisCacheMock = new Mock<IRedisCache>();
        var urlServiceMock = new Mock<IUrlService>();
        var vehicleSizeServiceMock = new Mock<IVehicleSizeService>();

        var request = new Mock<HttpRequest>();
        request.Setup(x => x.Host).Returns(HostString.FromUriComponent("http://localhost:8080"));
        request.Setup(x => x.Path).Returns(PathString.FromUriComponent("/vehicleSizes/{0}"));

        var httpContext = Mock.Of<HttpContext>(x => x.Request == request.Object);
        var controllerContext = new ControllerContext()
        {
            HttpContext = httpContext
        };

        var id = Guid.NewGuid();
        var vehicleSizeDTO_Mock = Task<VehicleSizeDTO>.Factory.StartNew(() =>
        {
            return new VehicleSizeDTO()
            {
                Id = id,
                Description = "Van",
                Active = It.IsAny<bool>()
            };
        });
        redisCacheMock.Setup(x => x.GetRecordAsync<VehicleSizeDTO>(It.IsAny<string>())).Returns(vehicleSizeDTO_Mock);

        var href_Mock = string.Format("https://localhost:8080/Valeting/vehicleSizes/{0}", id);
        urlServiceMock.Setup(x => x.GenerateSelf(It.IsAny<string>(), It.IsAny<string>())).Returns(href_Mock);

        //Act
        var vehicleSizeController = new VehicleSizeController(redisCacheMock.Object, vehicleSizeServiceMock.Object, urlServiceMock.Object)
        {
            ControllerContext = controllerContext
        };

        var objResult = (ObjectResult)vehicleSizeController.FindByIdAsync(id.ToString()).ConfigureAwait(false).GetAwaiter().GetResult();
        var response = (VehicleSizeApiResponse)objResult.Value;

        //Assert
        Assert.NotNull(objResult);
        Assert.Equal(200, objResult.StatusCode);
        Assert.True(objResult.Value.GetType() == typeof(VehicleSizeApiResponse));
        Assert.NotNull(response);
        Assert.True(response.VehicleSize.Id.Equals(id));
        Assert.Equal("Van", response.VehicleSize.Description);
        Assert.False(response.VehicleSize.Active);
        Assert.NotNull(response.VehicleSize.Link.Self);
        Assert.False(string.IsNullOrEmpty(response.VehicleSize.Link.Self.Href));
        Assert.Contains(string.Format("/Valeting/vehicleSizes/{0}", id), response.VehicleSize.Link.Self.Href);
    }

    [Fact]
    public void FindById_Status400()
    {
        //Arrange
        var redisCacheMock = new Mock<IRedisCache>();
        var urlServiceMock = new Mock<IUrlService>();
        var vehicleSizeServiceMock = new Mock<IVehicleSizeService>();

        vehicleSizeServiceMock.Setup(x => x.FindByIDAsync(It.IsAny<Guid>())).Throws(new InputException(It.IsAny<string>()));

        //Act
        var vehicleSizeController = new VehicleSizeController(redisCacheMock.Object, vehicleSizeServiceMock.Object, urlServiceMock.Object);
        var objResult = (ObjectResult)vehicleSizeController.FindByIdAsync(string.Format("{0}", Guid.Empty)).ConfigureAwait(false).GetAwaiter().GetResult();
        var response = (VehicleSizeApiError)objResult.Value;

        //Assert
        Assert.NotNull(objResult);
        Assert.Equal(400, objResult.StatusCode);
        Assert.NotNull(response);
        Assert.False(string.IsNullOrEmpty(response.Detail));
    }

    [Fact]
    public void FindById_Status404()
    {
        //Arrange
        var redisCacheMock = new Mock<IRedisCache>();
        var urlServiceMock = new Mock<IUrlService>();
        var vehicleSizeServiceMock = new Mock<IVehicleSizeService>();

        var id = Guid.NewGuid();
        vehicleSizeServiceMock.Setup(x => x.FindByIDAsync(id)).Throws(new NotFoundException(It.IsAny<string>()));

        //Act
        var vehicleSizeController = new VehicleSizeController(redisCacheMock.Object, vehicleSizeServiceMock.Object, urlServiceMock.Object);
        var objResult = (ObjectResult)vehicleSizeController.FindByIdAsync(id.ToString()).ConfigureAwait(false).GetAwaiter().GetResult();
        var response = (VehicleSizeApiError)objResult.Value;

        //Assert
        Assert.NotNull(objResult);
        Assert.Equal(404, objResult.StatusCode);
        Assert.NotNull(response);
        Assert.False(string.IsNullOrEmpty(response.Detail));
    }

    [Fact]
    public void FindById_Status500()
    {
        //Act
        var redisCacheMock = new Mock<IRedisCache>();
        var urlServiceMock = new Mock<IUrlService>();
        var vehicleSizeServiceMock = new Mock<IVehicleSizeService>();

        var vehicleSizeController = new VehicleSizeController(redisCacheMock.Object, vehicleSizeServiceMock.Object, urlServiceMock.Object);
        var objResult = (ObjectResult)vehicleSizeController.FindByIdAsync(null).ConfigureAwait(false).GetAwaiter().GetResult();
        var response = (VehicleSizeApiError)objResult.Value;

        //Assert
        Assert.NotNull(objResult);
        Assert.Equal(500, objResult.StatusCode);
        Assert.NotNull(response);
        Assert.False(string.IsNullOrEmpty(response.Detail));
    }

    [Fact]
    public void ListAll_Status200_WithCache()
    {
        //Arrange
        var redisCacheMock = new Mock<IRedisCache>();
        var urlServiceMock = new Mock<IUrlService>();
        var vehicleSizeServiceMock = new Mock<IVehicleSizeService>();

        var request = new Mock<HttpRequest>();
        request.Setup(x => x.Host).Returns(HostString.FromUriComponent("http://localhost:8080"));
        request.Setup(x => x.Path).Returns(PathString.FromUriComponent("/Valeting/vehicleSizes"));

        var httpContext = Mock.Of<HttpContext>(x => x.Request == request.Object);
        var controllerContext = new ControllerContext()
        {
            HttpContext = httpContext
        };

        var vehicleSizeListDTO_Mock = Task<VehicleSizeListDTO>.Factory.StartNew(() =>
        {
            return new VehicleSizeListDTO()
            {
                VehicleSizes = new List<VehicleSizeDTO>()
                {
                    new VehicleSizeDTO() { Id = Guid.NewGuid(), Description = "Van", Active = It.IsAny<bool>() },
                    new VehicleSizeDTO() { Id = Guid.NewGuid(), Description = "Small", Active = It.IsAny<bool>() },
                    new VehicleSizeDTO() { Id = Guid.NewGuid(), Description = "Medium", Active = It.IsAny<bool>() }
                },
                TotalItems = 3,
                TotalPages = 1
            };
        });
        redisCacheMock.Setup(x => x.GetRecordAsync<VehicleSizeListDTO>(It.IsAny<string>())).Returns(vehicleSizeListDTO_Mock);

        var linkDTO_Mock = new LinkDTO()
        {
            Next = string.Format("https://localhost:8080/Valeting/vehicleSizes?pageNumber={0}&pageSize={1}", 1, 2),
            Prev = string.Empty,
            Self = string.Format("https://localhost:8080/Valeting/vehicleSizes?pageNumber={0}&pageSize={1}", 1, 2),
        };
        urlServiceMock.Setup(x => x.GeneratePaginatedLinks(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<object>())).Returns(linkDTO_Mock);

        //Act

        //Assert
    }

    [Fact]
    public void ListAll_Status200_WithoutCache()
    {

    }

    [Fact]
    public void ListAll_Status400()
    {
        //Arrange
        var redisCacheMock = new Mock<IRedisCache>();
        var urlServiceMock = new Mock<IUrlService>();
        var vehicleSizeServiceMock = new Mock<IVehicleSizeService>();

        vehicleSizeServiceMock.Setup(x => x.ListAllAsync(It.IsAny<VehicleSizeFilterDTO>())).Throws(new InputException(It.IsAny<string>()));

        //Act
        var vehicleSizeController = new VehicleSizeController(redisCacheMock.Object, vehicleSizeServiceMock.Object, urlServiceMock.Object);
        var objResult = (ObjectResult)vehicleSizeController.ListAllAsync(new VehicleSizeApiParameters()).ConfigureAwait(false).GetAwaiter().GetResult();
        var response = (VehicleSizeApiError)objResult.Value;

        //Assert
        Assert.NotNull(objResult);
        Assert.Equal(400, objResult.StatusCode);
        Assert.NotNull(response);
        Assert.False(string.IsNullOrEmpty(response.Detail));
    }

    [Fact]
    public void ListAll_Status500()
    {
        //Arrange
        var redisCacheMock = new Mock<IRedisCache>();
        var urlServiceMock = new Mock<IUrlService>();
        var vehicleSizeServiceMock = new Mock<IVehicleSizeService>();

        //Act
        var vehicleSizeController = new VehicleSizeController(redisCacheMock.Object, vehicleSizeServiceMock.Object, urlServiceMock.Object);
        var objResult = (ObjectResult)vehicleSizeController.ListAllAsync(null).ConfigureAwait(false).GetAwaiter().GetResult();
        var response = (VehicleSizeApiError)objResult.Value;

        //Assert
        Assert.NotNull(objResult);
        Assert.Equal(500, objResult.StatusCode);
        Assert.NotNull(response);
        Assert.False(string.IsNullOrEmpty(response.Detail));
    }
}
