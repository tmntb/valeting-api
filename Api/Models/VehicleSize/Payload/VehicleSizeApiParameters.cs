using Api.Models.Core;

namespace Api.Models.VehicleSize.Payload;

/// <summary>
/// Represents the query string parameters for retrieving vehicle sizes with optional filtering and pagination.
/// </summary>
public class VehicleSizeApiParameters : QueryStringParametersApi
{
    /// <summary>
    /// Optional filter to return only active or inactive vehicle sizes.
    /// </summary>
    public bool? Active { get; set; }
}
