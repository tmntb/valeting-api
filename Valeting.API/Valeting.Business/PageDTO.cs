using System.ComponentModel.DataAnnotations;

namespace Valeting.Business;

public class PageDTO
{
    [Display(Name = "pageNumber", Order = 1)]
    public int PageNumber { get; set; }
    [Display(Name = "pageSize", Order = 2)]
    public int PageSize { get; set; }
}