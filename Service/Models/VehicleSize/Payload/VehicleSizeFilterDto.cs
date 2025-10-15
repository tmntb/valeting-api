using System.ComponentModel.DataAnnotations;
using Service.Models.Core;

namespace Service.Models.VehicleSize.Payload;

public class VehicleSizeFilterDto : FilterDto
{
    [Display(Name = "active", Order = 3)]
    public bool? Active { get; set; }
}