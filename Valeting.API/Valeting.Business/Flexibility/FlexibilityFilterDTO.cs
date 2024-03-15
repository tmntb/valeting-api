using System.ComponentModel.DataAnnotations;

namespace Valeting.Business.Flexibility;

public class FlexibilityFilterDTO : PageDTO
{
    [Display(Name = "active", Order = 3)]
    public bool? Active { get; set; }
}