using Valeting.API.Models.Core;

namespace Valeting.API.Models.VehicleSize;

public class VehicleSizeApiParameters : QueryStringParametersApi
{
    public bool? Active { get; set; }
}