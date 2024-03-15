using System.ComponentModel.DataAnnotations;

namespace Valeting.Services.Objects.Core;

public class PageSV
{
    [Display(Name = "pageNumber", Order = 1)]
    public int PageNumber { get; set; }
    
    [Display(Name = "pageSize", Order = 2)]
    public int PageSize { get; set; }
}