using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api.SwaggerDocumentation.Document;

/// <summary>
/// Custom Swagger/OpenAPI document filter for the Flexibility endpoints.
/// </summary>
/// <remarks>
/// This filter adds descriptive metadata for all flexibility-related operations,
/// including listing all flexibilities and retrieving a flexibility by ID.
/// It sets operation IDs, summaries, and descriptions for improved API documentation.
/// </remarks>
public class FlexibilityDocumentFilter : IDocumentFilter
{
    /// <summary>
    /// Endpoint for the general flexibilities collection.
    /// </summary>
    public const string FlexibilitiesEndpoint = "/flexibilities";

    /// <summary>
    /// Endpoint for a single flexibility by its identifier.
    /// </summary>
    public const string FlexibilitiesIdEndpoint = "/flexibilities/{id}";

    /// <summary>
    /// Applies the filter to the given OpenAPI document.
    /// </summary>
    /// <param name="swaggerDoc">The OpenAPI document to modify.</param>
    /// <param name="context">The document filter context.</param>
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        swaggerDoc.Tags.Add(new OpenApiTag() { Name = "Flexibility", Description = "Flexiblity operations" });

        var flexibilitiesPaths = swaggerDoc.Paths.FirstOrDefault(x => x.Key == FlexibilitiesEndpoint).Value;
        flexibilitiesPaths.Operations.FirstOrDefault(x => x.Key == OperationType.Get).Value.OperationId = "get-flexibilities";
        flexibilitiesPaths.Operations.FirstOrDefault(x => x.Key == OperationType.Get).Value.Summary = "List flexibilities";
        flexibilitiesPaths.Operations.FirstOrDefault(x => x.Key == OperationType.Get).Value.Description = "Return a list of all **Flexibilities**, it can be filter by the page number, page size and/or active";

        var flexibilitiesIdPaths = swaggerDoc.Paths.FirstOrDefault(x => x.Key == FlexibilitiesIdEndpoint).Value;
        flexibilitiesIdPaths.Operations.FirstOrDefault(x => x.Key == OperationType.Get).Value.OperationId = "get-flexibilities-id";
        flexibilitiesIdPaths.Operations.FirstOrDefault(x => x.Key == OperationType.Get).Value.Summary = "List flexibilities by id";
        flexibilitiesIdPaths.Operations.FirstOrDefault(x => x.Key == OperationType.Get).Value.Description = "Returns a **Flexibility** by the given id";
    }
}