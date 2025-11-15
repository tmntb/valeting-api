using System.ComponentModel.DataAnnotations;

namespace Api.SwaggerDocumentation.Parameter;

/// <summary>
/// Attribute used to provide metadata for path parameters in Swagger/OpenAPI documentation.
/// </summary>
/// <remarks>
/// Use this attribute on action method parameters to describe the parameter, provide an example value,
/// and specify a format for the parameter. This information will be picked up by the
/// <see cref="ParameterFilter"/> to enhance OpenAPI documentation.
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="PathParameterAttribute"/> class.
/// </remarks>
/// <param name="description">Description of the path parameter.</param>
/// <param name="example">Example value for the parameter.</param>
/// <param name="format">Format of the parameter.</param>
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
public class PathParameterAttribute(string description, string example, string format) : ValidationAttribute
{
    /// <summary>
    /// Description of the path parameter.
    /// </summary>
    public string Description { get; set; } = description;

    /// <summary>
    /// Example value for the path parameter.
    /// </summary>
    public string Example { get; set; } = example;

    /// <summary>
    /// Format of the path parameter (e.g., "uuid", "date").
    /// </summary>
    public string Format { get; set; } = format;
}
