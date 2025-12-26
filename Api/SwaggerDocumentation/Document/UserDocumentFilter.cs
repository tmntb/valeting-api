using System.Diagnostics.CodeAnalysis;
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
[ExcludeFromCodeCoverage]
public class UserDocumentFilter : IDocumentFilter
{
    /// <summary>
    /// Endpoint for user login.
    /// </summary>
    public const string UserLoginEndpoint = "/user/login";

    /// <summary>
    /// Endpoint for user register.
    /// </summary>
    public const string UserRegisterEndpoint = "/user/register";

    /// <summary>
    /// Endpoint to refresh user token
    /// </summary>
    public const string UserRefreshTokenEndpoint = "/user/refreshToken";

    /// <summary>
    /// Applies the filter to the given OpenAPI document.
    /// </summary>
    /// <param name="swaggerDoc">The OpenAPI document to modify.</param>
    /// <param name="context">The document filter context.</param>
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        swaggerDoc.Tags.Add(new OpenApiTag() { Name = "User", Description = "User operations" });

        var userLoginPaths = swaggerDoc.Paths.FirstOrDefault(x => x.Key == UserLoginEndpoint).Value;
        userLoginPaths.Operations.FirstOrDefault(x => x.Key == OperationType.Post).Value.OperationId = "post-login-user";
        userLoginPaths.Operations.FirstOrDefault(x => x.Key == OperationType.Post).Value.Summary = "Validates user credentials";
        userLoginPaths.Operations.FirstOrDefault(x => x.Key == OperationType.Post).Value.Description = "Returns an access token for the **User**";

        var userRegisterPaths = swaggerDoc.Paths.FirstOrDefault(x => x.Key == UserRegisterEndpoint).Value;
        userRegisterPaths.Operations.FirstOrDefault(x => x.Key == OperationType.Post).Value.OperationId= "post-register-user";
        userRegisterPaths.Operations.FirstOrDefault(x => x.Key == OperationType.Post).Value.Summary = "Register a new user";
        userRegisterPaths.Operations.FirstOrDefault(x => x.Key == OperationType.Post).Value.Description = string.Empty;

        var userRefreshTokenPaths = swaggerDoc.Paths.FirstOrDefault(x => x.Key == UserRefreshTokenEndpoint).Value;
        userRefreshTokenPaths.Operations.FirstOrDefault(x => x.Key == OperationType.Post).Value.OperationId= "post-refresh-token-user";
        userRefreshTokenPaths.Operations.FirstOrDefault(x => x.Key == OperationType.Post).Value.Summary = "Refresh the token for valid user";
        userRefreshTokenPaths.Operations.FirstOrDefault(x => x.Key == OperationType.Post).Value.Description = "Returns a refreshed access token for the **User**";
    }
}