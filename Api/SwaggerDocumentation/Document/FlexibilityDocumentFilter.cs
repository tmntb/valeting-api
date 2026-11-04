using System.Diagnostics.CodeAnalysis;
using Api.Extensions;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api.SwaggerDocumentation.Document;

/// <summary>
/// Custom Swagger/OpenAPI document filter for the Flexibility endpoints.
/// </summary>
[ExcludeFromCodeCoverage]
public class FlexibilityDocumentFilter : IDocumentFilter
{
    /// <summary>
    /// Endpoint for the general flexibilities collection.
    /// </summary>
    private const string FlexibilityEndpoint = "/api/v1/flexibility";

    /// <summary>
    /// Endpoint for a single flexibility by its identifier.
    /// </summary>
    private const string FlexibilityIdEndpoint = "/api/v1/flexibility/{id}";

    /// <summary>
    /// Applies the filter to the given OpenAPI document.
    /// </summary>
    /// <param name="swaggerDoc">The OpenAPI document to modify.</param>
    /// <param name="context">The document filter context.</param>
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        swaggerDoc.Tags.Add(new OpenApiTag() { Name = "Flexibility", Description = "Flexibility operations" });
        
        swaggerDoc.ConfigureOperation(FlexibilityEndpoint, HttpMethod.Get, "get-flexibilities",
            "List flexibilities", "Return a list of all **Flexibilities**, it can be filter by the page number, page size and/or active");

        swaggerDoc.ConfigureOperation(FlexibilityIdEndpoint, HttpMethod.Get, "get-flexibilities-id",
            "List flexibilities by id", "Returns a **Flexibility** by the given id");
    }
}