namespace Api.Models.Core;

/// <summary>
/// Represents an error response returned by the API.
/// </summary>
public class ErrorApi
{
    /// <summary>
    /// A detailed message describing the error.
    /// </summary>
    public string Detail { get; set; }
}
