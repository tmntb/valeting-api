using System.ComponentModel.DataAnnotations;
using Common.Models.Core;

namespace Common.Models.Flexibility;

public class FlexibilityFilterDto : FilterDto
{
    [Display(Name = "active", Order = 3)]
    public bool? Active { get; set; }
}