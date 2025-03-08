using FluentValidation;
using System.Net;
using System.Threading.Tasks;
using Valeting.API.Models.Core;
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
        httpContext.Response.StatusCode = GetStatusCode(exception);

        var isDevelopment = httpContext.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment();

        object errorResponse = httpContext.Response.StatusCode == (int)HttpStatusCode.InternalServerError
           ? new MiddlewareErrorResponse
           {
               ExceptionType = exception.GetType().FullName,
               Message = exception.Message,
               StackTrace = isDevelopment ? exception.StackTrace : null // Only in developement
           }
           : new ErrorApi { Detail = exception.Message };

        await httpContext.Response.WriteAsJsonAsync(errorResponse);
    }

    private static int GetStatusCode(Exception exception) => exception switch
    {
        ArgumentNullException or ArgumentException or ValidationException => (int)HttpStatusCode.BadRequest,
        InvalidOperationException => (int)HttpStatusCode.Conflict,
        UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
        KeyNotFoundException => (int)HttpStatusCode.NotFound,
        _ => (int)HttpStatusCode.InternalServerError
    };
}