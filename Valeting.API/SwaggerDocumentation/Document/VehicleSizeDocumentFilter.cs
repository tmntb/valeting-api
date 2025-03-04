using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace Valeting.SwaggerDocumentation.Document;

public class VehicleSizeDocumentFilter : IDocumentFilter
{
    public const string VehicleSizesEndpoint = "/vehicleSizes";
    public const string VehicleSizesIdEndpoint = "/vehicleSizes/{id}";

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