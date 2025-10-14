using Api.Models.Core;

namespace Api.Models.VehicleSize;

public class VehicleSizeApiParameters : QueryStringParametersApi
{
    public bool? Active { get; set; }
}