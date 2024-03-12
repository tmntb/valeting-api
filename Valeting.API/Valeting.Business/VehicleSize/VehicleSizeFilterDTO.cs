using System.ComponentModel.DataAnnotations;

namespace Valeting.Business.VehicleSize;

public class VehicleSizeFilterDTO : PageDTO
{
    [Display(Name = "active", Order = 3)]
    public bool? Active { get; set; }
}