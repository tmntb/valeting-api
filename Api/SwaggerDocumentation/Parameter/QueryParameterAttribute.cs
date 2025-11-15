namespace Api.SwaggerDocumentation.Parameter;

/// <summary>
/// Attribute used to provide metadata for query parameters in Swagger/OpenAPI documentation.
/// </summary>
/// <remarks>
/// Apply this attribute to properties representing query parameters to specify a description, example value,
/// minimum and maximum values, and optionally a format. This metadata will be used by the
/// <see cref="ParameterFilter"/> to generate enhanced OpenAPI documentation.
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class QueryParameterAttribute : Attribute
{
    /// <summary>
    /// Description of the query parameter.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Example value for the query parameter.
    /// </summary>
    public string Example { get; set; }

    /// <summary>
    /// Minimum allowed value for the query parameter.
    /// </summary>
    public int Minimum { get; set; }

    /// <summary>
    /// Maximum allowed value for the query parameter.
    /// </summary>
    public int Maximum { get; set; }

    /// <summary>
    /// Optional format for the query parameter (e.g., "int32", "date").
    /// </summary>
    public string Format { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryParameterAttribute"/> class with a minimum and maximum value.
    /// </summary>
    /// <param name="description">Description of the query parameter.</param>
    /// <param name="example">Example value for the query parameter.</param>
    /// <param name="minimum">Minimum allowed value.</param>
    /// <param name="maximum">Maximum allowed value.</param>
    public QueryParameterAttribute(string description, string example, int minimum, int maximum)
    {
        Description = description;
        Example = example;
        Minimum = minimum;
        Maximum = maximum;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryParameterAttribute"/> class with only a minimum value.
    /// </summary>
    /// <param name="description">Description of the query parameter.</param>
    /// <param name="example">Example value for the query parameter.</param>
    /// <param name="minimum">Minimum allowed value.</param>
    public QueryParameterAttribute(string description, string example, int minimum)
    {
        Description = description;
        Example = example;
        Minimum = minimum;
    }
}
