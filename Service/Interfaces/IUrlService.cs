using Common.Models.Link;

namespace Service.Interfaces;

public interface IUrlService
{
    GenerateSelfUrlDtoResponse GenerateSelf(GenerateSelfUrlDtoRequest generateSelfUrlDtoRequest);
    GeneratePaginatedLinksDtoResponse GeneratePaginatedLinks(GeneratePaginatedLinksDtoRequest generatePaginatedLinksDtoRequest);
}