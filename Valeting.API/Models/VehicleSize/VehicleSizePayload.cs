using Valeting.API.Models.Core;

namespace Valeting.API.Models.VehicleSize;

public class VehicleSizeApiPaginatedResponse : PaginationApi
{
    public List<VehicleSizeApi> VehicleSizes { get; set; }
}

public class VehicleSizeApiResponse
{
    public VehicleSizeApi VehicleSize { get; set; }
}

public class VehicleSizeApiError : ErrorApi { }