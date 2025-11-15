namespace Service.Models.Flexibility.Payload;

/// <summary>
/// Represents a paginated response for a list of flexibilities.
/// </summary>
public class FlexibilityPaginatedDtoResponse
{
    /// <summary>
    /// Total number of flexibilities available for the given filter.
    /// </summary>
    public int TotalItems { get; set; }

    /// <summary>
    /// Total number of pages based on the page size and total items.
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// List of flexibilities in the current page.
    /// </summary>
    public List<FlexibilityDto> Flexibilities { get; set; }
}
