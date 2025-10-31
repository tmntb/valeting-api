using System.ComponentModel.DataAnnotations;
using Service.Models.Core;

namespace Service.Models.Flexibility.Payload;

/// <summary>
/// Represents filtering criteria for retrieving flexibilities with optional pagination and active status.
/// </summary>
public class FlexibilityFilterDto : FilterDto
{
    /// <summary>
    /// Optional filter to retrieve only active or inactive flexibilities.
    /// </summary>
    [Display(Name = "active", Order = 3)]
    public bool? Active { get; set; }
}
