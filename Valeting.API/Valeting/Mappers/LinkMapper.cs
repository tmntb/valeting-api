using AutoMapper;
using Valeting.Models.Core;
using Valeting.Common.Models.Link;

namespace Valeting.Mappers;

public class LinkMapper : Profile
{
    public LinkMapper()
    {
        // Dto -> Api
        CreateMap<GeneratePaginatedLinksDtoResponse, PaginationLinksApi>()
            .ForMember(dest => dest.Next, opt => opt.MapFrom(src => new LinkApi { Href = src.Next } ))
            .ForMember(dest => dest.Prev, opt => opt.MapFrom(src => new LinkApi { Href = src.Prev } ))
            .ForMember(dest => dest.Self, opt => opt.MapFrom(src => new LinkApi { Href = src.Self } ));
    }
}