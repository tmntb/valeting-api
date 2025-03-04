using Valeting.Common.Models.VehicleSize;

namespace Valeting.Core.Interfaces;

public interface IVehicleSizeService
{
    Task<PaginatedVehicleSizeDtoResponse> GetAsync(PaginatedVehicleSizeDtoRequest paginatedVehicleSizeDtoRequest);
    Task<GetVehicleSizeDtoResponse> GetByIdAsync(GetVehicleSizeDtoRequest getVehicleSizeDtoRequest);
}