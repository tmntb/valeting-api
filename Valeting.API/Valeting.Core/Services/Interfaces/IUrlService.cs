using Valeting.Core.Models.Link;

namespace Valeting.Core.Services.Interfaces;

public interface IUrlService
{
    GenerateSelfUrlSVResponse GenerateSelf(GenerateSelfUrlSVRequest generateSelfUrlSVRequest);
    GeneratePaginatedLinksSVResponse GeneratePaginatedLinks(GeneratePaginatedLinksSVRequest generatePaginatedLinksSVRequest);
}