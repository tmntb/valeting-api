using Api.SwaggerDocumentation.Parameter;

namespace Api.Models.Core;

/// <summary>
/// Represents common query string parameters for paginated API requests.
/// </summary>
public abstract class QueryStringParametersApi
{
    /// <summary>
    /// Requested page number. Defaults to 1.
    /// </summary>
    [QueryParameter("The requested page number", "1", 1)]
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Number of items per page. Defaults to 10. Minimum is 1 and maximum is 10.
    /// </summary>
    [QueryParameter("The number of elements for the page request", "5", 1, 10)]
    public int PageSize { get; set; } = 10;
}
