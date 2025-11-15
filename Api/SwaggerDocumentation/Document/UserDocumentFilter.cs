using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api.SwaggerDocumentation.Document;

/// <summary>
/// Custom Swagger/OpenAPI document filter for the User endpoints.
/// </summary>
/// <remarks>
/// This filter adds descriptive metadata for user-related operations,
/// specifically the login endpoint. It sets operation IDs, summaries,
/// and descriptions to improve API documentation and clarity for consumers.
/// </remarks>
public class UserDocumentFilter : IDocumentFilter
{
    /// <summary>
    /// Endpoint for user login/verification.
    /// </summary>
    public const string UserVerifyEndpoint = "/user/login";

    /// <summary>
    /// Applies the filter to the given OpenAPI document.
    /// </summary>
    /// <param name="swaggerDoc">The OpenAPI document to modify.</param>
    /// <param name="context">The document filter context.</param>
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        swaggerDoc.Tags.Add(new OpenApiTag() { Name = "User", Description = "User operations" });

        var flexibilitiesPaths = swaggerDoc.Paths.FirstOrDefault(x => x.Key == UserVerifyEndpoint).Value;
        flexibilitiesPaths.Operations.FirstOrDefault(x => x.Key == OperationType.Post).Value.OperationId = "post-login-user";
        flexibilitiesPaths.Operations.FirstOrDefault(x => x.Key == OperationType.Post).Value.Summary = "Validates user credentials";
        flexibilitiesPaths.Operations.FirstOrDefault(x => x.Key == OperationType.Post).Value.Description = "Returns an access token for the **User**";
    }
}