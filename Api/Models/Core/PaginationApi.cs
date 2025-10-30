namespace Api.Models.Core;

/// <summary>
/// Represents the base structure for paginated API responses.
/// </summary>
public class PaginationApi
{
    /// <summary>
    /// The total number of items available across all pages.
    /// </summary>
    public int TotalItems { get; set; } = 1;

    /// <summary>
    /// The current page number.
    /// </summary>
    public int CurrentPage { get; set; } = 1;

    /// <summary>
    /// The total number of pages available.
    /// </summary>
    public int TotalPages { get; set; } = 1;

    /// <summary>
    /// Links to navigate between pages.
    /// </summary>
    public PaginationLinksApi Links { get; set; }
}
