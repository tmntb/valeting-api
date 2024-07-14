using System.ComponentModel.DataAnnotations;

using Valeting.Repository.Models.Core;

namespace Valeting.Repository.Models.Flexibility;

public class FlexibilityFilterDTO : PageDTO
{
    [Display(Name = "active", Order = 3)]
    public bool? Active { get; set; }
}