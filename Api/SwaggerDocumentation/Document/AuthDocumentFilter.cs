using System.Diagnostics.CodeAnalysis;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api.SwaggerDocumentation.Document;

/// <summary>
/// Custom Swagger/OpenAPI document filter for the Auth endpoints.
/// </summary>
[ExcludeFromCodeCoverage]
public class AuthDocumentFilter : IDocumentFilter
{
    /// <summary>
    /// Endpoint for user register.
    /// </summary>
    public const string AuthRegisterEndpoint = "/auth/register";

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var userRegisterPaths = swaggerDoc.Paths.FirstOrDefault(x => x.Key == AuthRegisterEndpoint).Value;
        userRegisterPaths.Operations.FirstOrDefault(x => x.Key == HttpMethod.Post).Value.OperationId = "post-register-user";
        userRegisterPaths.Operations.FirstOrDefault(x => x.Key == HttpMethod.Post).Value.Summary = "Register a new user";
        userRegisterPaths.Operations.FirstOrDefault(x => x.Key == HttpMethod.Post).Value.Description = string.Empty;
    }
}
