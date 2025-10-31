using System.ComponentModel.DataAnnotations;

namespace Service.Models.Core;

/// <summary>
/// Base class for filter parameters used in paginated requests.
/// </summary>
public class FilterDto
{
    /// <summary>
    /// The requested page number.
    /// </summary>
    [Display(Name = "pageNumber", Order = 1)]
    public int PageNumber { get; set; }

    /// <summary>
    /// The number of items per page.
    /// </summary>
    [Display(Name = "pageSize", Order = 2)]
    public int PageSize { get; set; }
}
