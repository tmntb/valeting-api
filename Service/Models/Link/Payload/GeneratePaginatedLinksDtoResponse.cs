namespace Service.Models.Link.Payload;

/// <summary>
/// Represents the generated URLs for paginated API responses.
/// </summary>
public class GeneratePaginatedLinksDtoResponse
{
    /// <summary>
    /// URL for the current page.
    /// </summary>
    public string Self { get; set; }

    /// <summary>
    /// URL for the previous page, if it exists; otherwise, an empty string.
    /// </summary>
    public string Prev { get; set; }

    /// <summary>
    /// URL for the next page, if it exists; otherwise, an empty string.
    /// </summary>
    public string Next { get; set; }
}
