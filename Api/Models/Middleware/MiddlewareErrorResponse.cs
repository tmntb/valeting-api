namespace Api.Models.Middleware;

/// <summary>
/// Represents detailed information about an exception for middleware responses.
/// </summary>
public class MiddlewareErrorResponse
{
    /// <summary>
    /// The fully qualified type name of the exception.
    /// </summary>
    public string ExceptionType { get; set; }

    /// <summary>
    /// The message describing the exception.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// The stack trace of the exception. Included only in development environments.
    /// </summary>
    public string StackTrace { get; set; }
}
