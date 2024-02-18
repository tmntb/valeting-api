using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using Valeting.Controllers;
using Valeting.Common.Exceptions;
using Valeting.Helpers.Interfaces;
using Valeting.Services.Interfaces;
using Valeting.Business.VehicleSize;
using Valeting.ApiObjects.VehicleSize;
using Valeting.Business;

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
        _vehicleSizeServiceMock.Setup(x => x.FindByIDAsync(id)).Returns(vehicleSizeDTO_Mock);

        var href_Mock = string.Format("https://localhost:8080/Valeting/vehicleSizes/{0}", id);
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
        Assert.IsTrue(response.VehicleSize.Link.Self.Href.Contains(string.Format("/Valeting/vehicleSizes/{0}", id)));
    }

    [TestMethod]
    public void FindById_Status200_WithCache()
    {
        //Arrange
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
        _redisCacheMock.Setup(x => x.GetRecordAsync<VehicleSizeDTO>(It.IsAny<string>())).Returns(vehicleSizeDTO_Mock);

        var href_Mock = string.Format("https://localhost:8080/Valeting/vehicleSizes/{0}", id);
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
        Assert.IsTrue(response.VehicleSize.Link.Self.Href.Contains(string.Format("/Valeting/vehicleSizes/{0}", id)));
    }

    [TestMethod]
    public void FindById_Status400()
    {
        //Arrange
        _vehicleSizeServiceMock.Setup(x => x.FindByIDAsync(It.IsAny<Guid>())).Throws(new InputException(It.IsAny<string>()));

        //Act
        var vehicleSizeController = new VehicleSizeController(_redisCacheMock.Object, _vehicleSizeServiceMock.Object, _urlServiceMock.Object);
        var objResult = (ObjectResult)vehicleSizeController.FindByIdAsync(string.Format("{0}", Guid.Empty)).ConfigureAwait(false).GetAwaiter().GetResult();
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

    [TestMethod]
    public void ListAll_Status200_WithCache()
    {
        //Arrange
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
        _redisCacheMock.Setup(x => x.GetRecordAsync<VehicleSizeListDTO>(It.IsAny<string>())).Returns(vehicleSizeListDTO_Mock);

        var linkDTO_Mock = new LinkDTO()
        {
            Next = string.Format("https://localhost:8080/Valeting/vehicleSizes?pageNumber={0}&pageSize={1}", 1, 2),
            Prev = string.Empty,
            Self = string.Format("https://localhost:8080/Valeting/vehicleSizes?pageNumber={0}&pageSize={1}", 1, 2),
        };        
        _urlServiceMock.Setup(x => x.GeneratePaginatedLinks(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<object>())).Returns(linkDTO_Mock);

         


        //Act

        //Assert
    }

    [TestMethod]
    public void ListAll_Status200_WithoutCache()
    {

    }

    [TestMethod]
    public void ListAll_Status400()
    {
        //Arrange
        _vehicleSizeServiceMock.Setup(x => x.ListAllAsync(It.IsAny<VehicleSizeFilterDTO>())).Throws(new InputException(It.IsAny<string>()));

        //Act
        var vehicleSizeController = new VehicleSizeController(_redisCacheMock.Object, _vehicleSizeServiceMock.Object, _urlServiceMock.Object);
        var objResult = (ObjectResult)vehicleSizeController.ListAllAsync(new VehicleSizeApiParameters()).ConfigureAwait(false).GetAwaiter().GetResult();
        var response = (VehicleSizeApiError)objResult.Value;

        //Assert
        Assert.IsNotNull(objResult);
        Assert.IsTrue(objResult.StatusCode == 400);
        Assert.IsNotNull(response);
        Assert.IsFalse(string.IsNullOrEmpty(response.Detail));
    }

    [TestMethod]
    public void ListAll_Status500()
    {
        //Arrange

        //Act
        var vehicleSizeController = new VehicleSizeController(_redisCacheMock.Object, _vehicleSizeServiceMock.Object, _urlServiceMock.Object);
        var objResult = (ObjectResult)vehicleSizeController.ListAllAsync(null).ConfigureAwait(false).GetAwaiter().GetResult();
        var response = (VehicleSizeApiError)objResult.Value;

        //Assert
        Assert.IsNotNull(objResult);
        Assert.IsTrue(objResult.StatusCode == 500);
        Assert.IsNotNull(response);
        Assert.IsFalse(string.IsNullOrEmpty(response.Detail));
    }
}
