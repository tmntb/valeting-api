using FluentValidation;
using Microsoft.AspNetCore.Http;
using Service.Interfaces;
using Service.Models.Core;
using Service.Models.Link.Payload;
using Service.Validators;
using Service.Validators.Utils;
using System.Web;

namespace Service.Services;

public class LinkService : ILinkService
{
    /// <inheritdoc />
    public string GenerateSelf(GenerateSelfLinkDtoRequest generateSelfLinkDtoRequest)
    {
        generateSelfLinkDtoRequest.ValidateRequest(new GenerateSelfLinkValidator());

        var baseUrl = BuildBaseUrl(generateSelfLinkDtoRequest.Request, generateSelfLinkDtoRequest.Path);
        return generateSelfLinkDtoRequest.Id == default ? baseUrl : $"{baseUrl}/{generateSelfLinkDtoRequest.Id}";
    }

    /// <inheritdoc />
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

    /// <summary>
    /// Builds the base URL for a given HTTP request, optionally replacing the path with a custom one.
    /// </summary>
    /// <param name="request">The current HTTP request.</param>
    /// <param name="customPath">Optional custom path to use instead of the request's path.</param>
    /// <returns>The fully constructed base URL including protocol, host, path base, and path.</returns>
    private static string BuildBaseUrl(HttpRequest request, string? customPath = null)
    {
        var protocol = request.Scheme;
        var host = request.Host.HasValue ? request.Host.Value : "localhost";
        var pathBase = request.PathBase.ToString().Trim('/');
        var path = string.IsNullOrEmpty(customPath) ? request.Path.ToString().TrimStart('/') : customPath;

        return $"{protocol}://{host}{(string.IsNullOrEmpty(pathBase) ? string.Empty : "/" + pathBase)}/{path}";
    }

    /// <summary>
    /// Generates a query string from a filter object, updating its page number.
    /// </summary>
    /// <param name="filterDto">The filter object containing query parameters.</param>
    /// <param name="newPageNumber">The new page number to include in the query string.</param>
    /// <returns>A URL-encoded query string representing the filter.</returns>
    private string GenerateQueryString(FilterDto filterDto, int newPageNumber)
    {
        var clonedFilter = CloneWithUpdatedPageNumber(filterDto, newPageNumber);

        return string.Join("&", clonedFilter.GetType().GetProperties()
                .Where(p => p.GetValue(clonedFilter, null) != null)
                .OrderBy(p => p.CustomAttributes.FirstOrDefault()?.NamedArguments.FirstOrDefault(i => i.MemberName.Equals("Order")).TypedValue.Value)
                .Select(p => $"{ToCamelCase(p.Name)}={HttpUtility.UrlEncode(FormatPropertyValue(p.GetValue(clonedFilter, null)))}"));
    }

    /// <summary>
    /// Clones a filter object and updates its PageNumber property.
    /// </summary>
    /// <param name="filterDto">The original filter object.</param>
    /// <param name="newPageNumber">The new page number to set in the clone.</param>
    /// <returns>A new filter object with the updated page number.</returns>
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

    /// <summary>
    /// Formats a property value for use in a query string.
    /// </summary>
    /// <param name="value">The value to format.</param>
    /// <returns>A string representation of the value, with booleans converted to "true" or "false".</returns>
    private static string FormatPropertyValue(object value) =>
        value switch
        {
            bool boolValue => boolValue ? "true" : "false",
            _ => value?.ToString() ?? string.Empty
        };

    /// <summary>
    /// Converts a string to camelCase.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The camelCase version of the string, or the original if null/empty.</returns>
    private static string ToCamelCase(string input) => string.IsNullOrEmpty(input) ? input : char.ToLowerInvariant(input[0]) + input.Substring(1);
}