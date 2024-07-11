using System.ComponentModel.DataAnnotations;

using Valeting.Core.Models.Core;

namespace Valeting.Core.Models.VehicleSize;

public class VehicleSizeFilterSV : PageSV
{
    [Display(Name = "active", Order = 3)]
    public bool? Active { get; set; }
} 