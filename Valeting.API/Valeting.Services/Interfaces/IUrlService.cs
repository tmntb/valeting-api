using Valeting.Services.Objects.Link;

namespace Valeting.Services.Interfaces;

public interface IUrlService
{
    GenerateSelfUrlSVResponse GenerateSelf(GenerateSelfUrlSVRequest generateSelfUrlSVRequest);
    GeneratePaginatedLinksSVResponse GeneratePaginatedLinks(GeneratePaginatedLinksSVRequest generatePaginatedLinksSVRequest);
}