using Service.Models.Link.Payload;

namespace Service.Interfaces;

public interface ILinkService
{
    string GenerateSelf(GenerateSelfLinkDtoRequest generateSelfLinkDtoRequest);
    GeneratePaginatedLinksDtoResponse GeneratePaginatedLinks(GeneratePaginatedLinksDtoRequest generatePaginatedLinksDtoRequest);
}