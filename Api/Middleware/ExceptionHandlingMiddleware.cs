using Api.Models.Core;
using Api.Models.Middleware;
using FluentValidation;
using Microsoft.IdentityModel.Tokens;
using System.Net;

namespace Api.Middleware;

public class ExceptionHandlingMiddleware : IMiddleware
{
    /// <summary>
    /// Invokes the middleware to process the HTTP context and handle exceptions.
    /// </summary>
    /// <param name="httpContext">The HTTP context for the current request.</param>
    /// <param name="next">The next middleware delegate in the pipeline.</param>
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

    /// <summary>
    /// Handles exceptions caught by the middleware and generates an appropriate HTTP response.
    /// </summary>
    /// <remarks>
    /// This method sets the <c>Content-Type</c> to <c>application/json</c> and assigns the correct
    /// HTTP status code based on the exception type.  
    /// For <c>500 Internal Server Error</c>, a <see cref="MiddlewareErrorResponse"/> is returned, including
    /// the exception type, message, and, if in development, the stack trace.  
    /// For other errors, an <see cref="ErrorApi"/> object is returned with a <c>Detail</c> message.
    /// </remarks>
    /// <param name="httpContext">The current HTTP context.</param>
    /// <param name="exception">The exception that was thrown.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
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

    /// <summary>
    /// Determines the appropriate HTTP status code for a given exception type.
    /// </summary>
    /// <param name="exception">The exception that occurred.</param>
    /// <returns>The corresponding HTTP status code.</returns>
    private static int GetStatusCode(Exception exception) => exception switch
    {
        ArgumentNullException or ArgumentException or ValidationException => (int)HttpStatusCode.BadRequest,
        InvalidOperationException => (int)HttpStatusCode.Conflict,
        UnauthorizedAccessException or SecurityTokenExpiredException => (int)HttpStatusCode.Unauthorized,
        KeyNotFoundException => (int)HttpStatusCode.NotFound,
        _ => (int)HttpStatusCode.InternalServerError
    };
}