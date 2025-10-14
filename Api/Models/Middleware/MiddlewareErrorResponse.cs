using System.Net;

namespace Api.Models.Middleware;

public class MiddlewareErrorResponse
{
    public string ExceptionType { get; set; }
    public string Message { get; set; }
    public string StackTrace { get; set; }
}
