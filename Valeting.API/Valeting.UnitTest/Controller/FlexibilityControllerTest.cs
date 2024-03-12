using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using Valeting.Helpers.Interfaces;
using Valeting.Services.Interfaces;

namespace Valeting.UnitTest.Controller;

[TestClass]
public class FlexibilityControllerTest
{
    private Mock<IRedisCache> _redisCacheMock;
    private Mock<IFlexibilityService> _flexibilityServiceMock;
    private Mock<IUrlService> _urlServiceMock;

    [TestInitialize]
    public void Init()
    {
        _redisCacheMock = new Mock<IRedisCache>();
        _flexibilityServiceMock = new Mock<IFlexibilityService>();
        _urlServiceMock = new Mock<IUrlService>();
    }

    [TestCleanup]
    public void Clean()
    {
        _redisCacheMock = null;
        _flexibilityServiceMock = null;
        _urlServiceMock = null;
    }
}