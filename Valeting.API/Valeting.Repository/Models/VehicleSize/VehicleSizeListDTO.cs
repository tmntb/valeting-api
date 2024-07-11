using Valeting.Repository.Models.Core;

namespace Valeting.Repository.Models.VehicleSize;

public class VehicleSizeListDTO : ContentDTO
{
    public List<VehicleSizeDTO> VehicleSizes { get; set; }
}