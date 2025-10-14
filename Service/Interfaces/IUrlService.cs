using Common.Models.Link;

namespace Service.Interfaces;

public interface IUrlService
{
    string GenerateSelf(GenerateSelfUrlDtoRequest generateSelfUrlDtoRequest);
    GeneratePaginatedLinksDtoResponse GeneratePaginatedLinks(GeneratePaginatedLinksDtoRequest generatePaginatedLinksDtoRequest);
}