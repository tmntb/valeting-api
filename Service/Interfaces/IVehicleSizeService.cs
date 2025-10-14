using Common.Models.VehicleSize;

namespace Service.Interfaces;

public interface IVehicleSizeService
{
    Task<PaginatedVehicleSizeDtoResponse> GetFilteredAsync(PaginatedVehicleSizeDtoRequest paginatedVehicleSizeDtoRequest);
    Task<GetVehicleSizeDtoResponse> GetByIdAsync(GetVehicleSizeDtoRequest getVehicleSizeDtoRequest);
}