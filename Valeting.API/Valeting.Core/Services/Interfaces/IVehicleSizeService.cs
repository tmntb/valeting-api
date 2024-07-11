using Valeting.Core.Models.VehicleSize;

namespace Valeting.Core.Services.Interfaces;

public interface IVehicleSizeService
{
    Task<PaginatedVehicleSizeSVResponse> GetAsync(PaginatedVehicleSizeSVRequest paginatedVehicleSizeSVRequest);
    Task<GetVehicleSizeSVResponse> GetByIdAsync(GetVehicleSizeSVRequest getVehicleSizeSVRequest);
}