using System.ComponentModel.DataAnnotations;

using Valeting.Services.Objects.Core;

namespace Valeting.Services.Objects.VehicleSize;

public class VehicleSizeFilterSV : PageSV
{
     [Display(Name = "active", Order = 3)]
    public bool? Active { get; set; }
} 