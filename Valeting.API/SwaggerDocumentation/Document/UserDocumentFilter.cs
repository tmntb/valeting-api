using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace Valeting.SwaggerDocumentation.Document;

public class UserDocumentFilter : IDocumentFilter
{
    public const string UserVerifyEndpoint = "/user/verify";

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        swaggerDoc.Tags.Add(new OpenApiTag() { Name = "User", Description = "User operations" });

        var flexibilitiesPaths = swaggerDoc.Paths.FirstOrDefault(x => x.Key == UserVerifyEndpoint).Value;
        flexibilitiesPaths.Operations.FirstOrDefault(x => x.Key == OperationType.Post).Value.OperationId = "post-verify-user";
        flexibilitiesPaths.Operations.FirstOrDefault(x => x.Key == OperationType.Post).Value.Summary = "Verify user credentials";
        flexibilitiesPaths.Operations.FirstOrDefault(x => x.Key == OperationType.Post).Value.Description = "Returns an access token for the **User**";
    }
}