using System.ComponentModel.DataAnnotations;
using Common.Models.Core;

namespace Common.Models.VehicleSize;

public class VehicleSizeFilterDto : FilterDto
{
    [Display(Name = "active", Order = 3)]
    public bool? Active { get; set; }
}