using Valeting.Common.Models.Core;

namespace Valeting.Common.Models.VehicleSize;

public class VehicleSizeListDto : ContentDto
{
    public List<VehicleSizeDto> VehicleSizes { get; set; }
}