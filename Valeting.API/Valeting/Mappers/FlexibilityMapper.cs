using AutoMapper;

using Valeting.Business.Flexibility;
using Valeting.Repositories.Entities;
using Valeting.ApiObjects.Flexibility;
using Valeting.Services.Objects.Flexibility;

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

        //Service -> Repository
        CreateMap<FlexibilityFilterSV, FlexibilityFilterDTO>();

        //Repository -> Service
        CreateMap<RdFlexibility, FlexibilityDTO>();

        //Service -> API
        CreateMap<FlexibilityDTO, FlexibilitySV>();
        CreateMap<FlexibilityListDTO, PaginatedFlexibilitySVResponse>();

        //API -> Out
        CreateMap<FlexibilitySV, FlexibilityApi>();
    }
}