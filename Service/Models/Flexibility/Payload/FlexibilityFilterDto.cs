using System.ComponentModel.DataAnnotations;
using Service.Models.Core;

namespace Service.Models.Flexibility.Payload;

public class FlexibilityFilterDto : FilterDto
{
    [Display(Name = "active", Order = 3)]
    public bool? Active { get; set; }
}