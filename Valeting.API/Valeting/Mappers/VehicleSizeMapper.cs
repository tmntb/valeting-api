using AutoMapper;

using Valeting.Business.VehicleSize;
using Valeting.Repositories.Entities;
using Valeting.ApiObjects.VehicleSize;
using Valeting.Services.Objects.VehicleSize;

namespace Valeting.Mappers;

public class VehicleSizeMapper : Profile
{
    public VehicleSizeMapper()
    {
        //API -> Service
        CreateMap<VehicleSizeApiParameters, PaginatedVehicleSizeSVRequest>()
            .ForMember(dest => dest.Filter, act => act.MapFrom(src => src));

        CreateMap<VehicleSizeApiParameters, VehicleSizeFilterSV>()
            .ForMember(dest => dest.PageNumber, opt => opt.MapFrom(src => src.PageNumber))
            .ForMember(dest => dest.PageSize, opt => opt.MapFrom(src => src.PageSize))
            .ForMember(dest => dest.Active, opt => opt.MapFrom(src => src.Active));

        //Service -> Repository
        CreateMap<VehicleSizeFilterSV, VehicleSizeFilterDTO>();

        //Repository -> Service
        CreateMap<RdVehicleSize, VehicleSizeDTO>();

        //Service -> API
        CreateMap<VehicleSizeDTO, VehicleSizeSV>();
        CreateMap<VehicleSizeListDTO, PaginatedVehicleSizeSVResponse>();

        //API -> Out
        CreateMap<VehicleSizeSV, VehicleSizeApi>();
    }
}
