using Valeting.Common.Models.Link;

namespace Valeting.Core.Interfaces;

public interface IUrlService
{
    GenerateSelfUrlDtoResponse GenerateSelf(GenerateSelfUrlDtoRequest generateSelfUrlDtoRequest);
    GeneratePaginatedLinksDtoResponse GeneratePaginatedLinks(GeneratePaginatedLinksDtoRequest generatePaginatedLinksDtoRequest);
}