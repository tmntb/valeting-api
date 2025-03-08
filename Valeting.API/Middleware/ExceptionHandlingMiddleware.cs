using FluentValidation;
using System.Net;
using Valeting.API.Models.Middleware;

namespace Valeting.API.Middleware;

public class ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger) : IMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
    {
        try
        {
            await next(httpContext);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(httpContext, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
    {
        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = exception switch
        {
            ArgumentNullException => (int)HttpStatusCode.BadRequest,
            ArgumentException => (int)HttpStatusCode.BadRequest,
            InvalidOperationException => (int)HttpStatusCode.Conflict,
            UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
            ValidationException => (int)HttpStatusCode.BadRequest,
            KeyNotFoundException => (int)HttpStatusCode.NotFound,
            _ => (int)HttpStatusCode.InternalServerError
        };

        await BuildErrorResponse(httpContext, exception);
    }

    private async Task BuildErrorResponse(HttpContext httpContext, Exception exception)
    {
        var status = Enum.IsDefined(typeof(HttpStatusCode), httpContext.Response.StatusCode.ToString()) ?
            httpContext.Response.StatusCode : (int)HttpStatusCode.InternalServerError;

        httpContext.Response.StatusCode = status;

        var isDevelopment = httpContext.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment();

        var errorResponse = new MiddlewareErrorResponse
        {
            StatusCode = (HttpStatusCode)status,
            ExceptionType = exception.GetType().FullName,
            Message = exception.Message,
            StackTrace = isDevelopment ? exception.StackTrace : null // Only in developement
        };

        _logger.LogError("Exception occurred - StatusCode: {StatusCode}, Type: {Type}, Message: {Message}",
            errorResponse.StatusCode, errorResponse.ExceptionType, errorResponse.Message);

        await httpContext.Response.WriteAsJsonAsync(errorResponse);
    }
}