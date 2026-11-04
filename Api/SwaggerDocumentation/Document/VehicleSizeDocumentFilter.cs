using System.Diagnostics.CodeAnalysis;
using Api.Extensions;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api.SwaggerDocumentation.Document;

/// <summary>
/// Custom Swagger/OpenAPI document filter for the VehicleSize endpoints.
/// </summary>
[ExcludeFromCodeCoverage]
public class VehicleSizeDocumentFilter : IDocumentFilter
{
    /// <summary>
    /// Endpoint for listing vehicle sizes.
    /// </summary>
    private const string VehicleSizeEndpoint = "/api/v1/vehiclesize";

    /// <summary>
    /// Endpoint for retrieving a vehicle size by ID.
    /// </summary>
    private const string VehicleSizeIdEndpoint = "/api/v1/vehiclesize/{id}";

    /// <summary>
    /// Applies the filter to the given OpenAPI document.
    /// </summary>
    /// <param name="swaggerDoc">The OpenAPI document to modify.</param>
    /// <param name="context">The document filter context.</param>
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        swaggerDoc.Tags.Add(new OpenApiTag() { Name = "VehicleSize", Description = "Vehicle size operations" });
        
        swaggerDoc.ConfigureOperation(VehicleSizeIdEndpoint, HttpMethod.Get, "get-vehiclesizes-id",
            "List vehicle sizes by id", "Returns a **Vehicle Size** by the given id");
        
        swaggerDoc.ConfigureOperation(VehicleSizeEndpoint, HttpMethod.Get, "get-vehiclesizes",
             "List vehicle sizes", "Return a list of all **Vehicle Sizes**, it can be filter by the page number, page size and/or active");
    }
}