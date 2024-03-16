using Valeting.Services.Objects.VehicleSize;

namespace Valeting.Services.Interfaces;

public interface IVehicleSizeService
{
    Task<GetVehicleSizeSVResponse> GetAsync(GetVehicleSizeSVRequest getVehicleSizeSVRequest);
    Task<PaginatedVehicleSizeSVResponse> ListAllAsync(PaginatedVehicleSizeSVRequest paginatedVehicleSizeSVRequest);
}