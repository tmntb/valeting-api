using Api.Models.Core;

namespace Api.Models.Flexibility.Payload;

/// <summary>
/// Paginated response containing a list of flexibilities.
/// </summary>
public class FlexibilityApiPaginatedResponse : PaginationApi
{
    /// <summary>
    /// List of flexibilities in the current page.
    /// </summary>
    public List<FlexibilityApi> Flexibilities { get; set; }
}
