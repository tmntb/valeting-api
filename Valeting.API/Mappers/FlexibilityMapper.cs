using AutoMapper;
using Valeting.Repository.Entities;
using Valeting.API.Models.Flexibility;
using Valeting.Common.Models.Flexibility;

namespace Valeting.API.Mappers;

public class FlexibilityMapper : Profile
{
    public FlexibilityMapper()
    {
        // Api -> Dto
        CreateMap<FlexibilityApiParameters, PaginatedFlexibilityDtoRequest>()
            .ForMember(dest => dest.Filter, act => act.MapFrom(src => src));

        CreateMap<FlexibilityApiParameters, FlexibilityFilterDto>()
            .ForMember(dest => dest.PageNumber, opt => opt.MapFrom(src => src.PageNumber))
            .ForMember(dest => dest.PageSize, opt => opt.MapFrom(src => src.PageSize))
            .ForMember(dest => dest.Active, opt => opt.MapFrom(src => src.Active));

        // Entity -> Dto
        CreateMap<RdFlexibility, FlexibilityDto>();

        // Dto -> Api
        CreateMap<FlexibilityDto, FlexibilityApi>();
    }
}