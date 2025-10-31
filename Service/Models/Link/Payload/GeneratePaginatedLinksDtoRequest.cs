using Microsoft.AspNetCore.Http;
using Service.Models.Core;

namespace Service.Models.Link.Payload;

/// <summary>
/// Represents the input data required to generate paginated links for an API response.
/// </summary>
public class GeneratePaginatedLinksDtoRequest
{
    /// <summary>
    /// Filter parameters including page number and page size used in the request.
    /// </summary>
    public FilterDto Filter { get; set; }

    /// <summary>
    /// Total number of pages available for the current filter.
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// The HTTP request from which the base URL and query string will be derived.
    /// </summary>
    public HttpRequest Request { get; set; }
}
