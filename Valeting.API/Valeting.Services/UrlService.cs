using System.Web;

using Valeting.Services.Interfaces;
using Valeting.Services.Objects.Link;

namespace Valeting.Services;

public class UrlService : IUrlService
{
    public GenerateSelfUrlSVResponse GenerateSelf(GenerateSelfUrlSVRequest generateSelfUrlSVRequest)
    {
        return new()
        {
            Self = generateSelfUrlSVRequest.Id == default ? 
                    string.Format("https://{0}{1}", generateSelfUrlSVRequest.BaseUrl, generateSelfUrlSVRequest.Path) : 
                    string.Format("https://{0}{1}/{2}", generateSelfUrlSVRequest.BaseUrl, generateSelfUrlSVRequest.Path, generateSelfUrlSVRequest.Id)
        };
    }

    public GeneratePaginatedLinksSVResponse GeneratePaginatedLinks(GeneratePaginatedLinksSVRequest generatePaginatedLinksSVRequest)
    {
        var generatePaginatedLinksSVResponse = new GeneratePaginatedLinksSVResponse()
        {
            Prev = string.Empty,
            Next = string.Empty,
            Self = string.Empty
        };

        if (generatePaginatedLinksSVRequest.PageNumber > 1)
        {
            var pg = generatePaginatedLinksSVRequest.Filter.GetType().GetProperty("PageNumber");
            pg.SetValue(generatePaginatedLinksSVRequest.Filter, generatePaginatedLinksSVRequest.PageNumber - 1);
            var queryStringStr = BuildQueryString(generatePaginatedLinksSVRequest.Filter);

            generatePaginatedLinksSVResponse.Prev = string.Format("https://{0}{1}?{2}", generatePaginatedLinksSVRequest.BaseUrl, generatePaginatedLinksSVRequest.Path, queryStringStr);
        }

        if (generatePaginatedLinksSVRequest.PageNumber < generatePaginatedLinksSVRequest.TotalPages)
        {
            var pg = generatePaginatedLinksSVRequest.Filter.GetType().GetProperty("PageNumber");
            pg.SetValue(generatePaginatedLinksSVRequest.Filter, generatePaginatedLinksSVRequest.PageNumber + 1);
            var queryStringStr = BuildQueryString(generatePaginatedLinksSVRequest.Filter);

            generatePaginatedLinksSVResponse.Next = string.Format("https://{0}{1}?{2}", generatePaginatedLinksSVRequest.BaseUrl, generatePaginatedLinksSVRequest.Path, queryStringStr);
        }

        generatePaginatedLinksSVResponse.Self = string.Format("https://{0}{1}{2}", generatePaginatedLinksSVRequest.BaseUrl, generatePaginatedLinksSVRequest.Path, generatePaginatedLinksSVRequest.QueryString);

        return generatePaginatedLinksSVResponse;
    }

    private string BuildQueryString(object filter)
    {
        var properties = from p in filter.GetType().GetProperties().OrderBy(y => y.CustomAttributes.FirstOrDefault().NamedArguments.FirstOrDefault(i => i.MemberName.Equals("Order")).TypedValue.Value)
                         where p.GetValue(filter, null) != null
                         select p.CustomAttributes.FirstOrDefault().NamedArguments.FirstOrDefault().TypedValue.Value + "=" + HttpUtility.UrlEncode(p.GetValue(filter, null).ToString());

        return string.Join("&", properties.ToArray());
    }
}