using FluentValidation;
using Microsoft.AspNetCore.Http;
using System.Web;
using Valeting.Common.Models.Link;
using Valeting.Core.Interfaces;
using Valeting.Core.Validators;
using Valeting.Core.Validators.Utils;

namespace Valeting.Core.Services;

public class UrlService : IUrlService
{
    public GenerateSelfUrlDtoResponse GenerateSelf(GenerateSelfUrlDtoRequest generateSelfUrlDtoRequest)
    {
        generateSelfUrlDtoRequest.ValidateRequest(new GenerateSelfUrlValidator());

        var request = generateSelfUrlDtoRequest.Request;
        var fullBaseUrl = GetFullBaseUrl(request);
        var path = !string.IsNullOrEmpty(generateSelfUrlDtoRequest.Path) ? generateSelfUrlDtoRequest.Path : request.Path.ToString().TrimStart('/');

        return new()
        {
            Self = generateSelfUrlDtoRequest.Id == default ? $"{fullBaseUrl}/{path}" : $"{fullBaseUrl}/{path}/{generateSelfUrlDtoRequest.Id}"
        };
    }

    public GeneratePaginatedLinksDtoResponse GeneratePaginatedLinks(GeneratePaginatedLinksDtoRequest generatePaginatedLinksDtoRequest)
    {
        generatePaginatedLinksDtoRequest.ValidateRequest(new GeneratePaginatedLinksValidator());

        var request = generatePaginatedLinksDtoRequest.Request;
        var fullBaseUrl = GetFullBaseUrl(request);
        var path = request.Path.ToString().TrimStart('/');
        var queryString = request.QueryString.HasValue ? request.QueryString.Value : string.Empty;

        return new()
        {
            Self = $"{fullBaseUrl}/{path}{queryString}",
            Prev = generatePaginatedLinksDtoRequest.PageNumber > 1 ?
                $"{fullBaseUrl}/{path}?{GenerateQueryString(generatePaginatedLinksDtoRequest.Filter, generatePaginatedLinksDtoRequest.PageNumber - 1)}" : string.Empty,
            Next = generatePaginatedLinksDtoRequest.PageNumber < generatePaginatedLinksDtoRequest.TotalPages ?
                $"{fullBaseUrl}/{path}?{GenerateQueryString(generatePaginatedLinksDtoRequest.Filter, generatePaginatedLinksDtoRequest.PageNumber + 1)}" : string.Empty
        };
    }

    private static string GetFullBaseUrl(HttpRequest request)
    {
        var protocol = request.Scheme;
        var baseUrl = request.Host.HasValue ? request.Host.Value : "localhost";
        var pathBase = request.PathBase.ToString().Trim('/');

        return $"{protocol}://{baseUrl}{(string.IsNullOrEmpty(pathBase) ? string.Empty : "/" + pathBase)}";
    }

    private string GenerateQueryString(object filter, int newPageNumber)
    {
        filter.GetType().GetProperty("PageNumber")?.SetValue(filter, newPageNumber);

        var properties = from p in filter.GetType().GetProperties().OrderBy(y => y.CustomAttributes.FirstOrDefault()?.NamedArguments.FirstOrDefault(i => i.MemberName.Equals("Order")).TypedValue.Value)
                         where p.GetValue(filter, null) != null
                         select p.CustomAttributes.FirstOrDefault()?.NamedArguments.FirstOrDefault().TypedValue.Value + "=" + HttpUtility.UrlEncode(FormatPropertyValue(p.GetValue(filter, null)));

        return string.Join("&", properties.ToArray());
    }

    private static string FormatPropertyValue(object value) =>
        value switch
        {
            bool boolValue => boolValue ? "true" : "false",
            _ => value?.ToString() ?? string.Empty
        };
}