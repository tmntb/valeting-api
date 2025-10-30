namespace Api.Models.Core;

/// <summary>
/// Represents the navigation links for a paginated API response.
/// </summary>
public class PaginationLinksApi
{
    /// <summary>
    /// The link to the previous page, if available.
    /// </summary>
    public LinkApi Prev { get; set; }

    /// <summary>
    /// The link to the next page, if available.
    /// </summary>
    public LinkApi Next { get; set; }

    /// <summary>
    /// The link to the current page (self).
    /// </summary>
    public LinkApi Self { get; set; }
}
