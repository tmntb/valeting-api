using System.Web;
using Valeting.Common.Models.Link;
using Valeting.Core.Interfaces;

namespace Valeting.Core.Services;

public class UrlService : IUrlService
{
    public GenerateSelfUrlDtoResponse GenerateSelf(GenerateSelfUrlDtoRequest generateSelfUrlDtoRequest)
    {
        return new()
        {
            Self = generateSelfUrlDtoRequest.Id == default ? 
                    string.Format("https://{0}{1}", generateSelfUrlDtoRequest.BaseUrl, generateSelfUrlDtoRequest.Path) : 
                    string.Format("https://{0}{1}/{2}", generateSelfUrlDtoRequest.BaseUrl, generateSelfUrlDtoRequest.Path, generateSelfUrlDtoRequest.Id)
        };
    }

    public GeneratePaginatedLinksDtoResponse GeneratePaginatedLinks(GeneratePaginatedLinksDtoRequest generatePaginatedLinksDtoRequest)
    {
        var generatePaginatedLinksDtoResponse = new GeneratePaginatedLinksDtoResponse()
        {
            Prev = string.Empty,
            Next = string.Empty,
            Self = string.Empty
        };

        if (generatePaginatedLinksDtoRequest.PageNumber > 1)
        {
            var pg = generatePaginatedLinksDtoRequest.Filter.GetType().GetProperty("PageNumber");
            pg.SetValue(generatePaginatedLinksDtoRequest.Filter, generatePaginatedLinksDtoRequest.PageNumber - 1);
            var queryStringStr = BuildQueryString(generatePaginatedLinksDtoRequest.Filter);

            generatePaginatedLinksDtoResponse.Prev = string.Format("https://{0}{1}?{2}", generatePaginatedLinksDtoRequest.BaseUrl, generatePaginatedLinksDtoRequest.Path, queryStringStr);
        }

        if (generatePaginatedLinksDtoRequest.PageNumber < generatePaginatedLinksDtoRequest.TotalPages)
        {
            var pg = generatePaginatedLinksDtoRequest.Filter.GetType().GetProperty("PageNumber");
            pg.SetValue(generatePaginatedLinksDtoRequest.Filter, generatePaginatedLinksDtoRequest.PageNumber + 1);
            var queryStringStr = BuildQueryString(generatePaginatedLinksDtoRequest.Filter);

            generatePaginatedLinksDtoResponse.Next = string.Format("https://{0}{1}?{2}", generatePaginatedLinksDtoRequest.BaseUrl, generatePaginatedLinksDtoRequest.Path, queryStringStr);
        }

        generatePaginatedLinksDtoResponse.Self = string.Format("https://{0}{1}{2}", generatePaginatedLinksDtoRequest.BaseUrl, generatePaginatedLinksDtoRequest.Path, generatePaginatedLinksDtoRequest.QueryString);

        return generatePaginatedLinksDtoResponse;
    }

    private string BuildQueryString(object filter)
    {
        var properties = from p in filter.GetType().GetProperties().OrderBy(y => 
                            y.CustomAttributes.FirstOrDefault()?.NamedArguments.FirstOrDefault(
                                i => i.MemberName.Equals("Order")).TypedValue.Value)
                          where p.GetValue(filter, null) != null
                          select p.CustomAttributes.FirstOrDefault()?.NamedArguments.FirstOrDefault().TypedValue.Value 
                            + "=" + HttpUtility.UrlEncode(FormatPropertyValue(p.GetValue(filter, null)));

        return string.Join("&", properties.ToArray());
    }

    private string FormatPropertyValue(object value)
    {
        return value is bool boolValue ? boolValue ? "true" : "false" : value.ToString();
    }
}