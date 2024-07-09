using AutoMapper;

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

        //Service -> Entity

        //Entity -> Service

        //Service -> API
        CreateMap<VehicleSizeSV, VehicleSizeApi>();
    }
}
