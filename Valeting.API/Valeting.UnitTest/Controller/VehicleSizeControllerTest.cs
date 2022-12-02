using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Valeting.ApiObjects.VehicleSize;
using Valeting.Business.VehicleSize;
using Valeting.Common.Exceptions;
using Valeting.Controllers;
using Valeting.Helpers.Interfaces;
using Valeting.Services.Interfaces;

namespace Valeting.UnitTest.Controller;

[TestClass]
public class VehicleSizeControllerTest
{
    private Mock<IRedisCache> _redisCacheMock;
    private Mock<IVehicleSizeService> _vehicleSizeServiceMock;
    private Mock<IUrlService> _urlServiceMock;

    [TestInitialize]
    public void Init()
    {
        _redisCacheMock = new Mock<IRedisCache>();
        _vehicleSizeServiceMock = new Mock<IVehicleSizeService>();
        _urlServiceMock = new Mock<IUrlService>();
    }

    [TestCleanup]
    public void Clean()
    {
        _redisCacheMock = null;
        _vehicleSizeServiceMock = null;
        _urlServiceMock = null;
    }

    [TestMethod]
    public void FindById_Status200_WithoutCache()
    {
        //Arrange
        var request = new Mock<HttpRequest>();
        request.Setup(x => x.Host).Returns(HostString.FromUriComponent("http://localhost:8080"));
        request.Setup(x => x.Path).Returns(PathString.FromUriComponent("/vehicleSizes"));

        var httpContext = Mock.Of<HttpContext>(x => x.Request == request.Object);
        var controllerContext = new ControllerContext()
        {
            HttpContext = httpContext,
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
        _vehicleSizeServiceMock.Setup(x => x.FindByIDAsync(id)).Returns(vehicleSizeDTO_Mock);

        var href_Mock = string.Format("https://localhost:8080/vehicleSizes/{0}", id);
        _urlServiceMock.Setup(x => x.GenerateSelf(It.IsAny<string>(), It.IsAny<string>())).Returns(href_Mock);

        //Act
        var vehicleSizeController = new VehicleSizeController(_redisCacheMock.Object, _vehicleSizeServiceMock.Object, _urlServiceMock.Object)
        {
            ControllerContext = controllerContext
        };

        var objResult = (ObjectResult)vehicleSizeController.FindByIdAsync(id.ToString()).ConfigureAwait(false).GetAwaiter().GetResult();
        var response = (VehicleSizeApiResponse)objResult.Value;

        //Assert
        Assert.IsNotNull(objResult);
        Assert.IsTrue(objResult.StatusCode == 200);
        Assert.IsTrue(objResult.Value.GetType() == typeof(VehicleSizeApiResponse));
        Assert.IsNotNull(response);
        Assert.IsTrue(response.VehicleSize.Id.Equals(id));
        Assert.IsTrue(response.VehicleSize.Description.Equals("Van"));
        Assert.IsFalse(response.VehicleSize.Active);
        Assert.IsNotNull(response.VehicleSize.Link.Self);
        Assert.IsFalse(string.IsNullOrEmpty(response.VehicleSize.Link.Self.Href));
        Assert.IsTrue(response.VehicleSize.Link.Self.Href.Contains(string.Format("/vehicleSizes/{0}", id)));
    }

    [TestMethod]
    public void FindById_Status200_WithCache()
    {
        //Arrange
        var request = new Mock<HttpRequest>();
        request.Setup(x => x.Host).Returns(HostString.FromUriComponent("http://localhost:8080"));
        request.Setup(x => x.Path).Returns(PathString.FromUriComponent("/vehicleSizes"));

        var httpContext = Mock.Of<HttpContext>(x => x.Request == request.Object);
        var controllerContext = new ControllerContext()
        {
            HttpContext = httpContext,
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
        _redisCacheMock.Setup(x => x.GetRecordAsync<VehicleSizeDTO>(It.IsAny<string>())).Returns(vehicleSizeDTO_Mock);

        var href_Mock = string.Format("https://localhost:8080/vehicleSizes/{0}", id);
        _urlServiceMock.Setup(x => x.GenerateSelf(It.IsAny<string>(), It.IsAny<string>())).Returns(href_Mock);

        //Act
        var vehicleSizeController = new VehicleSizeController(_redisCacheMock.Object, _vehicleSizeServiceMock.Object, _urlServiceMock.Object)
        {
            ControllerContext = controllerContext
        };

        var objResult = (ObjectResult)vehicleSizeController.FindByIdAsync(id.ToString()).ConfigureAwait(false).GetAwaiter().GetResult();
        var response = (VehicleSizeApiResponse)objResult.Value;

        //Assert
        Assert.IsNotNull(objResult);
        Assert.IsTrue(objResult.StatusCode == 200);
        Assert.IsTrue(objResult.Value.GetType() == typeof(VehicleSizeApiResponse));
        Assert.IsNotNull(response);
        Assert.IsTrue(response.VehicleSize.Id.Equals(id));
        Assert.IsTrue(response.VehicleSize.Description.Equals("Van"));
        Assert.IsFalse(response.VehicleSize.Active);
        Assert.IsNotNull(response.VehicleSize.Link.Self);
        Assert.IsFalse(string.IsNullOrEmpty(response.VehicleSize.Link.Self.Href));
        Assert.IsTrue(response.VehicleSize.Link.Self.Href.Contains(string.Format("/vehicleSizes/{0}", id)));
    }

    [TestMethod]
    public void FindById_Status400()
    {
        //Arrange
        var id = Guid.NewGuid();
        _vehicleSizeServiceMock.Setup(x => x.FindByIDAsync(id)).Throws(new InputException(It.IsAny<string>()));

        //Act
        var vehicleSizeController = new VehicleSizeController(_redisCacheMock.Object, _vehicleSizeServiceMock.Object, _urlServiceMock.Object);
        var objResult = (ObjectResult)vehicleSizeController.FindByIdAsync(id.ToString()).ConfigureAwait(false).GetAwaiter().GetResult();
        var response = (VehicleSizeApiError)objResult.Value;

        //Assert
        Assert.IsNotNull(objResult);
        Assert.IsTrue(objResult.StatusCode == 400);
        Assert.IsNotNull(response);
        Assert.IsFalse(string.IsNullOrEmpty(response.Detail));
    }

    [TestMethod]
    public void FindById_Status404()
    {
        //Arrange
        var id = Guid.NewGuid();
        _vehicleSizeServiceMock.Setup(x => x.FindByIDAsync(id)).Throws(new NotFoundException(It.IsAny<string>()));

        //Act
        var vehicleSizeController = new VehicleSizeController(_redisCacheMock.Object, _vehicleSizeServiceMock.Object, _urlServiceMock.Object);
        var objResult = (ObjectResult)vehicleSizeController.FindByIdAsync(id.ToString()).ConfigureAwait(false).GetAwaiter().GetResult();
        var response = (VehicleSizeApiError)objResult.Value;

        //Assert
        Assert.IsNotNull(objResult);
        Assert.IsTrue(objResult.StatusCode == 404);
        Assert.IsNotNull(response);
        Assert.IsFalse(string.IsNullOrEmpty(response.Detail));
    }

    [TestMethod]
    public void FindById_Status500()
    {
        //Arrange

        //Act
        var vehicleSizeController = new VehicleSizeController(_redisCacheMock.Object, _vehicleSizeServiceMock.Object, _urlServiceMock.Object);
        var objResult = (ObjectResult)vehicleSizeController.FindByIdAsync(null).ConfigureAwait(false).GetAwaiter().GetResult();
        var response = (VehicleSizeApiError)objResult.Value;

        //Assert
        Assert.IsNotNull(objResult);
        Assert.IsTrue(objResult.StatusCode == 500);
        Assert.IsNotNull(response);
        Assert.IsFalse(string.IsNullOrEmpty(response.Detail));
    }
}
