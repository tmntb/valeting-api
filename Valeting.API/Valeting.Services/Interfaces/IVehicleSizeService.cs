using Valeting.Services.Objects.VehicleSize;

namespace Valeting.Services.Interfaces;

public interface IVehicleSizeService
{
    Task<PaginatedVehicleSizeSVResponse> GetAsync(PaginatedVehicleSizeSVRequest paginatedVehicleSizeSVRequest);
    Task<GetVehicleSizeSVResponse> GetByIdAsync(GetVehicleSizeSVRequest getVehicleSizeSVRequest);
}