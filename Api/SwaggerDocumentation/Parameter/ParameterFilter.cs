using System.Diagnostics.CodeAnalysis;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api.SwaggerDocumentation.Parameter;

/// <summary>
/// Custom Swagger/OpenAPI parameter filter that applies metadata from
/// <see cref="QueryParameterAttribute"/> and <see cref="PathParameterAttribute"/> to parameters.
/// </summary>
/// <remarks>
/// This filter enhances OpenAPI documentation for query and path parameters
/// by adding descriptions, examples, and constraints such as minimum/maximum values or format.
/// </remarks>
[ExcludeFromCodeCoverage]
public class ParameterFilter : IParameterFilter
{
    /// <summary>
    /// Applies the filter to a given OpenAPI parameter.
    /// </summary>
    /// <param name="parameter">The OpenAPI parameter to modify.</param>
    /// <param name="context">The context of the parameter being processed.</param>
    public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
    {
        if (context.PropertyInfo != null)
        {
            var attributes = context.PropertyInfo.GetCustomAttributes(true);
            IEnumerable<QueryParameterAttribute>? queryParameterAttributes = attributes.OfType<QueryParameterAttribute>();
            if (queryParameterAttributes != null && queryParameterAttributes.Any())
                AddExample(parameter, queryParameterAttributes);
        }

        if (context.ParameterInfo != null)
        {
            var attributes = context.ParameterInfo.GetCustomAttributes(true);
            IEnumerable<PathParameterAttribute>? pathParameterAttributes = attributes.OfType<PathParameterAttribute>();
            if (pathParameterAttributes != null && pathParameterAttributes.Any())
                AddExample(parameter, pathParameterAttributes);
        }
    }

    /// <summary>
    /// Adds example, description, minimum, and maximum values to an OpenAPI query parameter.
    /// </summary>
    /// <param name="parameter">The OpenAPI parameter to modify.</param>
    /// <param name="parameterAttributes">The query parameter attributes to read metadata from.</param>
    private void AddExample(OpenApiParameter parameter, IEnumerable<QueryParameterAttribute> parameterAttributes)
    {
        foreach (var item in parameterAttributes)
        {
            parameter.Description = item.Description;
            parameter.Schema.Example = new OpenApiString(item.Example);
            parameter.Schema.Minimum = item.Minimum;
            if (item.Maximum != 0)
                parameter.Schema.Maximum = item.Maximum;
        }
    }

    /// <summary>
    /// Adds example, description, and format to an OpenAPI path parameter.
    /// </summary>
    /// <param name="parameter">The OpenAPI parameter to modify.</param>
    /// <param name="parameterAttributes">The path parameter attributes to read metadata from.</param>
    private void AddExample(OpenApiParameter parameter, IEnumerable<PathParameterAttribute> parameterAttributes)
    {
        foreach (var item in parameterAttributes)
        {
            parameter.Description = item.Description;
            parameter.Schema.Example = new OpenApiString(item.Example);
            parameter.Schema.Format = item.Format;
        }
    }
}
