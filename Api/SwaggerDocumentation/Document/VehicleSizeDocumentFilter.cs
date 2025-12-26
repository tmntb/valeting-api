using System.Diagnostics.CodeAnalysis;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api.SwaggerDocumentation.Document;

/// <summary>
/// Custom Swagger/OpenAPI document filter for the VehicleSize endpoints.
/// </summary>
/// <remarks>
/// This filter adds descriptive metadata for vehicle size operations,
/// including listing all vehicle sizes and retrieving a vehicle size by ID.
/// It sets operation IDs, summaries, and descriptions to improve API documentation clarity.
/// </remarks>
[ExcludeFromCodeCoverage]
public class VehicleSizeDocumentFilter : IDocumentFilter
{
    /// <summary>
    /// Endpoint for listing vehicle sizes.
    /// </summary>
    public const string VehicleSizesEndpoint = "/vehicleSizes";

    /// <summary>
    /// Endpoint for retrieving a vehicle size by ID.
    /// </summary>
    public const string VehicleSizesIdEndpoint = "/vehicleSizes/{id}";

    /// <summary>
    /// Applies the filter to the given OpenAPI document.
    /// </summary>
    /// <param name="swaggerDoc">The OpenAPI document to modify.</param>
    /// <param name="context">The document filter context.</param>
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        swaggerDoc.Tags.Add(new OpenApiTag() { Name = "VehicleSize", Description = "Vehicle size operations" });

        var flexibilitiesPaths = swaggerDoc.Paths.FirstOrDefault(x => x.Key == VehicleSizesEndpoint).Value;
        flexibilitiesPaths.Operations.FirstOrDefault(x => x.Key == OperationType.Get).Value.OperationId = "get-vehiclesizes";
        flexibilitiesPaths.Operations.FirstOrDefault(x => x.Key == OperationType.Get).Value.Summary = "List vehicle sizes";
        flexibilitiesPaths.Operations.FirstOrDefault(x => x.Key == OperationType.Get).Value.Description = "Return a list of all **Vehicle Sizes**, it can be filter by the page number, page size and/or active";

        var flexibilitiesIdPaths = swaggerDoc.Paths.FirstOrDefault(x => x.Key == VehicleSizesIdEndpoint).Value;
        flexibilitiesIdPaths.Operations.FirstOrDefault(x => x.Key == OperationType.Get).Value.OperationId = "get-vehiclesizes-id";
        flexibilitiesIdPaths.Operations.FirstOrDefault(x => x.Key == OperationType.Get).Value.Summary = "List vehicle sizes by id";
        flexibilitiesIdPaths.Operations.FirstOrDefault(x => x.Key == OperationType.Get).Value.Description = "Returns a **Vehicle Size** by the given id";
    }
}