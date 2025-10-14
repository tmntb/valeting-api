using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api.SwaggerDocumentation.Document;

public class UserDocumentFilter : IDocumentFilter
{
    public const string UserVerifyEndpoint = "/user/login";

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        swaggerDoc.Tags.Add(new OpenApiTag() { Name = "User", Description = "User operations" });

        var flexibilitiesPaths = swaggerDoc.Paths.FirstOrDefault(x => x.Key == UserVerifyEndpoint).Value;
        flexibilitiesPaths.Operations.FirstOrDefault(x => x.Key == OperationType.Post).Value.OperationId = "post-login-user";
        flexibilitiesPaths.Operations.FirstOrDefault(x => x.Key == OperationType.Post).Value.Summary = "Validates user credentials";
        flexibilitiesPaths.Operations.FirstOrDefault(x => x.Key == OperationType.Post).Value.Description = "Returns an access token for the **User**";
    }
}