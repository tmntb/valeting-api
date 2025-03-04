using System.Net;

namespace Valeting.Models.Middleware;

public class MiddlewareErrorResponse
{
    public HttpStatusCode StatusCode { get; set; }
    public string ExceptionType { get; set; }
    public string Message { get; set; }
    public string StackTrace { get; set; }
}
