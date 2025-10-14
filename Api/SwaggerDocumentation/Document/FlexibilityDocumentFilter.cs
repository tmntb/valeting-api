using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api.SwaggerDocumentation.Document;

public class FlexibilityDocumentFilter : IDocumentFilter
{
    public const string FlexibilitiesEndpoint = "/flexibilities";
    public const string FlexibilitiesIdEndpoint = "/flexibilities/{id}";

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