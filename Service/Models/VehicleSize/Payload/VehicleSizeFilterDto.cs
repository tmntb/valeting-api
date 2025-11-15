using Service.Models.Core;
using System.ComponentModel.DataAnnotations;

namespace Service.Models.VehicleSize.Payload;

/// <summary>
/// Filter criteria for querying vehicle sizes with pagination support.
/// </summary>
public class VehicleSizeFilterDto : FilterDto
{
    /// <summary>
    /// Optional filter to return only active or inactive vehicle sizes.
    /// </summary>
    [Display(Name = "active", Order = 3)]
    public bool? Active { get; set; }
}
