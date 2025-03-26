using System.ComponentModel.DataAnnotations;
using Valeting.Common.Models.Core;

namespace Valeting.Common.Models.Flexibility;

public class FlexibilityFilterDto : FilterDto
{
    [Display(Name = "active", Order = 3)]
    public bool? Active { get; set; }
}