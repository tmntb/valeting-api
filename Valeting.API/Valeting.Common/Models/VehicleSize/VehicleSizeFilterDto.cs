using System.ComponentModel.DataAnnotations;
using Valeting.Common.Models.Core;

namespace Valeting.Common.Models.VehicleSize;

public class VehicleSizeFilterDto : PageDto
{
    [Display(Name = "active", Order = 3)]
    public bool? Active { get; set; }
}