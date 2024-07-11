using AutoMapper;

using Valeting.Repository.Models.Flexibility;
using Valeting.Repository.Entities;
using Valeting.ApiObjects.Flexibility;
using Valeting.Core.Models.Flexibility;

namespace Valeting.Mappers;

public class FlexibilityMapper : Profile
{
    public FlexibilityMapper()
    {
        //API -> Service
        CreateMap<FlexibilityApiParameters, PaginatedFlexibilitySVRequest>()
            .ForMember(dest => dest.Filter, act => act.MapFrom(src => src));

        CreateMap<FlexibilityApiParameters, FlexibilityFilterSV>()
            .ForMember(dest => dest.PageNumber, opt => opt.MapFrom(src => src.PageNumber))
            .ForMember(dest => dest.PageSize, opt => opt.MapFrom(src => src.PageSize))
            .ForMember(dest => dest.Active, opt => opt.MapFrom(src => src.Active));

        //Service -> DTO
        CreateMap<FlexibilityFilterSV, FlexibilityFilterDTO>();

        //Entity -> DTO
        CreateMap<RdFlexibility, FlexibilityDTO>();

        //DTO -> Service
        CreateMap<FlexibilityDTO, FlexibilitySV>();
        CreateMap<FlexibilityListDTO, PaginatedFlexibilitySVResponse>();

        //Service -> API
        CreateMap<FlexibilitySV, FlexibilityApi>();
    }
}