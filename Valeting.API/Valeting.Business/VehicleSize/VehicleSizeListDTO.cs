using Valeting.Business.Core;

namespace Valeting.Business.VehicleSize;

public class VehicleSizeListDTO : ContentDTO
{
    public List<VehicleSizeDTO> VehicleSizes { get; set; }
}