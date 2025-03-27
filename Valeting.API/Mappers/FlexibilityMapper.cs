using AutoMapper;
using Valeting.API.Models.Flexibility;
using Valeting.Common.Models.Flexibility;
using Valeting.Repository.Entities;

namespace Valeting.API.Mappers;

public class FlexibilityMapper : Profile
{
    public FlexibilityMapper()
    {
        #region Api -> Dto
        CreateMap<FlexibilityApiParameters, PaginatedFlexibilityDtoRequest>()
            .ForMember(dest => dest.Filter, act => act.MapFrom(src => src));

        CreateMap<FlexibilityApiParameters, FlexibilityFilterDto>()
            .ForMember(dest => dest.PageNumber, opt => opt.MapFrom(src => src.PageNumber))
            .ForMember(dest => dest.PageSize, opt => opt.MapFrom(src => src.PageSize))
            .ForMember(dest => dest.Active, opt => opt.MapFrom(src => src.Active));
        #endregion

        #region Entity -> Dto
        CreateMap<RdFlexibility, FlexibilityDto>();
        #endregion

        #region Dto -> Api
        CreateMap<FlexibilityDto, FlexibilityApi>();
        #endregion
    }
}