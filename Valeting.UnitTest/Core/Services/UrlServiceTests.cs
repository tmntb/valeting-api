using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using Valeting.Common.Models.Core;
using Valeting.Core.Services;

namespace Valeting.Tests.Core.Services;

public class UrlServiceTests
{
    private readonly Guid _mockId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private readonly UrlService _service;

    public UrlServiceTests()
    {
        _service = new UrlService();
    }

    [Fact]
    public void GenerateSelf_ShouldReturnSelfUrl_WhenDtoPathSet()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Scheme = "https";
        context.Request.Host = new HostString("api");
        context.Request.PathBase = "/test";

        // Act
        var result = _service.GenerateSelf(new()
        {
            Id = _mockId,
            Path = "path",
            Request = context.Request
        });

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Self);
        Assert.Equal($"https://api/test/path/{_mockId}", result.Self);
    }

    [Fact]
    public void GenerateSelf_ShouldReturnSelfUrl_WhenDtoPathNotSet()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Scheme = "https";
        context.Request.Path = "/rPath";

        // Act
        var result = _service.GenerateSelf(new()
        {
            Id = _mockId,
            Request = context.Request
        });

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Self);
        Assert.Equal($"https://localhost/rPath/{_mockId}", result.Self);
    }

    [Fact]
    public void GeneratePaginatedLinks_ShouldReturnOnlySelfLink()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Scheme = "https";
        context.Request.Host = new HostString("api");
        context.Request.PathBase = "/test";
        context.Request.Path = "/path";
        context.Request.QueryString = new QueryString("?pageNumber=1&pageSize=1&active=false");

        // Act
        var result = _service.GeneratePaginatedLinks(new()
        {
            Request = context.Request,
            TotalPages = 1,
            Filter = new TestFilter
            {
                PageNumber = 1,
                PageSize = 1,
                Active = false
            }
        });

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Self);
        Assert.Equal($"https://api/test/path?pageNumber=1&pageSize=1&active=false", result.Self);
        Assert.Empty(result.Prev);
        Assert.Empty(result.Next);
    }

    [Fact]
    public void GeneratePaginatedLinks_ShouldReturnSelfAndNextLinks()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Scheme = "https";
        context.Request.Host = new HostString("api");
        context.Request.PathBase = "/test";
        context.Request.Path = "/path";
        context.Request.QueryString = new QueryString("?pageNumber=1&pageSize=1&active=true");

        // Act
        var result = _service.GeneratePaginatedLinks(new()
        {
            Request = context.Request,
            TotalPages = 4,
            Filter = new TestFilter
            {
                PageNumber = 1,
                PageSize = 1,
                Active = true
            }
        });

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Self);
        Assert.Equal($"https://api/test/path?pageNumber=1&pageSize=1&active=true", result.Self);
        Assert.Empty(result.Prev);
        Assert.NotEmpty(result.Next);
        Assert.Equal($"https://api/test/path?pageNumber=2&pageSize=1&active=true", result.Next);
    }

    [Fact]
    public void GeneratePaginatedLinks_ShouldReturnSelfPrevLinks()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Scheme = "https";
        context.Request.Host = new HostString("api");
        context.Request.PathBase = "/test";
        context.Request.Path = "/path";
        context.Request.QueryString = new QueryString("?pageNumber=4&pageSize=1&active=true");

        // Act
        var result = _service.GeneratePaginatedLinks(new()
        {
            Request = context.Request,
            TotalPages = 4,
            Filter = new TestFilter
            {
                PageNumber = 4,
                PageSize = 1,
                Active = true
            }
        });

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Self);
        Assert.Equal($"https://api/test/path?pageNumber=4&pageSize=1&active=true", result.Self);
        Assert.NotEmpty(result.Prev);
        Assert.Equal($"https://api/test/path?pageNumber=3&pageSize=1&active=true", result.Prev);
        Assert.Empty(result.Next);
    }

    [Fact]
    public void GeneratePaginatedLinks_ShouldReturnSelfAndNextAndPrevLinks()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Scheme = "https";
        context.Request.Host = new HostString("api");
        context.Request.PathBase = "/test";
        context.Request.Path = "/path";
        context.Request.QueryString = new QueryString("?pageNumber=2&pageSize=1&active=true");

        // Act
        var result = _service.GeneratePaginatedLinks(new()
        {
            Request = context.Request,
            TotalPages = 4,
            Filter = new TestFilter
            {
                PageNumber = 2,
                PageSize = 1,
                Active = true
            }
        });

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Self);
        Assert.Equal($"https://api/test/path?pageNumber=2&pageSize=1&active=true", result.Self);
        Assert.NotEmpty(result.Prev);
        Assert.Equal($"https://api/test/path?pageNumber=1&pageSize=1&active=true", result.Prev);
        Assert.NotEmpty(result.Next);
        Assert.Equal($"https://api/test/path?pageNumber=3&pageSize=1&active=true", result.Next);
    }

    private class TestFilter : FilterDto 
    {
        [Display(Name = "active", Order = 3)]
        public bool Active { get; set; }
    }
}
