using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using System.Text.Json;
using Api.Middleware;
using Api.Models.Core;
using Api.Models.Middleware;

namespace Api.Tests.Middleware;

public class ExceptionHandlingMiddlewareTests
{
    private readonly Mock<ILogger<ExceptionHandlingMiddleware>> _mockLogger;
    private readonly Mock<IWebHostEnvironment> _mockEnvironment;

    private readonly ExceptionHandlingMiddleware _middleware;

    public ExceptionHandlingMiddlewareTests()
    {
        _mockLogger = new Mock<ILogger<ExceptionHandlingMiddleware>>();
        _mockEnvironment = new Mock<IWebHostEnvironment>();

        _mockEnvironment.Setup(e => e.EnvironmentName).Returns(Environments.Development);

        _middleware = new ExceptionHandlingMiddleware(_mockLogger.Object);
    }

    [Fact]
    public async Task InvokeAsync_NoException_CallsNextDelegate()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var nextDelegate = new Mock<RequestDelegate>();

        // Act
        await _middleware.InvokeAsync(context, nextDelegate.Object);

        // Assert
        nextDelegate.Verify(next => next(It.IsAny<HttpContext>()), Times.Once);
    }

    [Theory]
    [InlineData(typeof(ArgumentNullException), HttpStatusCode.BadRequest)]
    [InlineData(typeof(ArgumentException), HttpStatusCode.BadRequest)]
    [InlineData(typeof(ValidationException), HttpStatusCode.BadRequest)]
    [InlineData(typeof(InvalidOperationException), HttpStatusCode.Conflict)]
    [InlineData(typeof(UnauthorizedAccessException), HttpStatusCode.Unauthorized)]
    [InlineData(typeof(KeyNotFoundException), HttpStatusCode.NotFound)]
    public async Task InvokeAsync_ExceptionThrown_ReturnsExpectedStatusCode(Type exceptionType, HttpStatusCode expectedStatusCode)
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var exception = exceptionType == typeof(ValidationException) ? 
            new ValidationException("Validation failed") : 
            (Exception)Activator.CreateInstance(exceptionType)!;

        var services = new ServiceCollection();
        services.AddSingleton(_mockEnvironment.Object);
        context.RequestServices = services.BuildServiceProvider();

        Task nextDelegate(HttpContext _) => throw exception;

        // Act
        await _middleware.InvokeAsync(context, nextDelegate);

        // Assert
        Assert.Equal((int)expectedStatusCode, context.Response.StatusCode);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseContent = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var errorResponse = JsonSerializer.Deserialize<ErrorApi>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(errorResponse);
        Assert.Equal(exception.Message, errorResponse.Detail);
    }

    [Fact]
    public async Task InvokeAsync_InternalServerError_DevelopmentMode_IncludesStackTrace()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var exception = new Exception("Test exception");

        var services = new ServiceCollection();
        services.AddSingleton(_mockEnvironment.Object);
        context.RequestServices = services.BuildServiceProvider();

        Task nextDelegate(HttpContext _) => throw exception;

        // Act
        await _middleware.InvokeAsync(context, nextDelegate);

        // Assert
        Assert.Equal((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseContent = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var errorResponse = JsonSerializer.Deserialize<MiddlewareErrorResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(errorResponse);
        Assert.Equal(exception.GetType().FullName, errorResponse.ExceptionType);
        Assert.Equal(exception.Message, errorResponse.Message);
        Assert.NotNull(errorResponse.StackTrace);
    }
}
