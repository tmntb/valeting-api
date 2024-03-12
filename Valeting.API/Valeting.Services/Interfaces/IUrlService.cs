using Valeting.Business;

namespace Valeting.Services.Interfaces;

public interface IUrlService
{
    string GenerateSelf(string baseUrl, string path);
    string GenerateSelf(string baseUrl, string path, Guid id);
    LinkDTO GeneratePaginatedLinks(string baseUrl, string path, string queryString, int pageNumber, int totalPages, object filter);
}