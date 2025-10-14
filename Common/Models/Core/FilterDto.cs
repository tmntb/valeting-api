using System.ComponentModel.DataAnnotations;

namespace Common.Models.Core;

public class FilterDto
{
    [Display(Name = "pageNumber", Order = 1)]
    public int PageNumber { get; set; }
    [Display(Name = "pageSize", Order = 2)]
    public int PageSize { get; set; }
}