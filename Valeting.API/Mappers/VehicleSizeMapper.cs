using AutoMapper;
using Valeting.Models.VehicleSize;
using Valeting.Repository.Entities;
using Valeting.Common.Models.VehicleSize;

namespace Valeting.Mappers;

public class VehicleSizeMapper : Profile
{
    public VehicleSizeMapper()
    {
        // Api -> Dto
        CreateMap<VehicleSizeApiParameters, PaginatedVehicleSizeDtoRequest>()
            .ForMember(dest => dest.Filter, act => act.MapFrom(src => src));

        CreateMap<VehicleSizeApiParameters, VehicleSizeFilterDto>()
            .ForMember(dest => dest.PageNumber, opt => opt.MapFrom(src => src.PageNumber))
            .ForMember(dest => dest.PageSize, opt => opt.MapFrom(src => src.PageSize))
            .ForMember(dest => dest.Active, opt => opt.MapFrom(src => src.Active));

        // Entity -> Dto
        CreateMap<RdVehicleSize, VehicleSizeDto>();

        // Dto -> Api
        CreateMap<VehicleSizeDto, VehicleSizeApi>();
    }
}
