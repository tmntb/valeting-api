using Valeting.Repository.Models.Core;
using System.ComponentModel.DataAnnotations;

namespace Valeting.Repository.Models.VehicleSize;

public class VehicleSizeFilterDTO : PageDTO
{
    [Display(Name = "active", Order = 3)]
    public bool? Active { get; set; }
}