using Common.Models.Core;
using Common.Models.Link;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Service.Interfaces;
using Service.Validators;
using Service.Validators.Utils;
using System.Web;

namespace Service.Services;

public class UrlService : IUrlService
{
    public string GenerateSelf(GenerateSelfUrlDtoRequest generateSelfUrlDtoRequest)
    {
        generateSelfUrlDtoRequest.ValidateRequest(new GenerateSelfUrlValidator());

        var baseUrl = BuildBaseUrl(generateSelfUrlDtoRequest.Request, generateSelfUrlDtoRequest.Path);
        return generateSelfUrlDtoRequest.Id == default ? baseUrl : $"{baseUrl}/{generateSelfUrlDtoRequest.Id}";
    }

    public GeneratePaginatedLinksDtoResponse GeneratePaginatedLinks(GeneratePaginatedLinksDtoRequest generatePaginatedLinksDtoRequest)
    {
        generatePaginatedLinksDtoRequest.ValidateRequest(new GeneratePaginatedLinksValidator());

        var baseUrl = BuildBaseUrl(generatePaginatedLinksDtoRequest.Request);
        var filter = generatePaginatedLinksDtoRequest.Filter;

        return new()
        {
            Self = $"{baseUrl}{generatePaginatedLinksDtoRequest.Request.QueryString}",
            Prev = filter.PageNumber > 1 ? $"{baseUrl}?{GenerateQueryString(filter, filter.PageNumber - 1)}" : string.Empty,
            Next = filter.PageNumber < generatePaginatedLinksDtoRequest.TotalPages ? $"{baseUrl}?{GenerateQueryString(filter, filter.PageNumber + 1)}" : string.Empty
        };
    }

    private static string BuildBaseUrl(HttpRequest request, string? customPath = null)
    {
        var protocol = request.Scheme;
        var host = request.Host.HasValue ? request.Host.Value : "localhost";
        var pathBase = request.PathBase.ToString().Trim('/');
        var path = string.IsNullOrEmpty(customPath) ? request.Path.ToString().TrimStart('/') : customPath;

        return $"{protocol}://{host}{(string.IsNullOrEmpty(pathBase) ? string.Empty : "/" + pathBase)}/{path}";
    }

    private string GenerateQueryString(FilterDto filterDto, int newPageNumber)
    {
        var clonedFilter = CloneWithUpdatedPageNumber(filterDto, newPageNumber);

        return string.Join("&", clonedFilter.GetType().GetProperties()
                .Where(p => p.GetValue(clonedFilter, null) != null)
                .OrderBy(p => p.CustomAttributes.FirstOrDefault()?.NamedArguments.FirstOrDefault(i => i.MemberName.Equals("Order")).TypedValue.Value)
                .Select(p => $"{ToCamelCase(p.Name)}={HttpUtility.UrlEncode(FormatPropertyValue(p.GetValue(clonedFilter, null)))}"));
    }

    private static object CloneWithUpdatedPageNumber(FilterDto filterDto, int newPageNumber)
    {
        var clonedFilter = Activator.CreateInstance(filterDto.GetType());

        foreach (var prop in filterDto.GetType().GetProperties().Where(p => p.CanRead && p.CanWrite))
        {
            object value = prop.Name == "PageNumber" ? newPageNumber : prop.GetValue(filterDto);
            prop.SetValue(clonedFilter, value);
        }

        return clonedFilter;
    }

    private static string FormatPropertyValue(object value) =>
        value switch
        {
            bool boolValue => boolValue ? "true" : "false",
            _ => value?.ToString() ?? string.Empty
        };

    private static string ToCamelCase(string input) => string.IsNullOrEmpty(input) ? input : char.ToLowerInvariant(input[0]) + input.Substring(1);
}