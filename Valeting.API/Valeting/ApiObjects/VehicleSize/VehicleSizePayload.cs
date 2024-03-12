namespace Valeting.ApiObjects.VehicleSize;

public class VehicleSizeApiPaginatedResponse : PaginationApi
{
    public List<VehicleSizeApi> VehicleSizes { get; set; }
}

public class VehicleSizeApiResponse
{
    public VehicleSizeApi VehicleSize { get; set; }
}

public class VehicleSizeApiError : ErrorApi { }