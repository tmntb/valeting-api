using Valeting.Models.Core;

namespace Valeting.Models.VehicleSize;

public class VehicleSizeApiParameters : QueryStringParametersApi
{
    public bool? Active { get; set; }
}