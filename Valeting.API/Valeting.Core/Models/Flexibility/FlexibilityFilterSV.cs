using System.ComponentModel.DataAnnotations;

using Valeting.Core.Models.Core;

namespace Valeting.Core.Models.Flexibility;

public class FlexibilityFilterSV : PageSV
{
    [Display(Name = "active", Order = 3)]
    public bool? Active { get; set; }
} 