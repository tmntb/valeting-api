using System.Diagnostics.CodeAnalysis;
using Api.Extensions;
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
    /// Endpoint for forgot password.
    /// </summary>
    private const string AuthForgotPasswordEndpoint = "/api/v1/auth/forgot-password";

    /// <summary>
    /// Endpoint for login.
    /// </summary>
    private const string AuthLoginEndpoint = "/api/v1/auth/login";

    /// <summary>
    /// Endpoint for auth mfa regenerate recovery codes.
    /// </summary>
    private const string AuthMfaRecoveryCodeRegenerate = "/api/v1/auth/recovery-codes";

    /// <summary>
    /// Endpoint to refresh user token
    /// </summary>
    private const string AuthRefreshTokenEndpoint = "/api/v1/auth/refresh-token";

    /// <summary>
    /// Endpoint for user register.
    /// </summary>
    private const string AuthRegisterEndpoint = "/api/v1/auth/register";

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        swaggerDoc.Tags.Add(new OpenApiTag() { Name = "Auth", Description = "Auth operations" });

        swaggerDoc.ConfigureOperation(AuthForgotPasswordEndpoint, HttpMethod.Post, "post-auth-forgot-password", 
            "Restores the user password.", "Restores the new **password** set by the user.", false);

        swaggerDoc.ConfigureOperation(AuthLoginEndpoint, HttpMethod.Post, "post-auth-login",
            "Validates user credentials.", "Returns an access token for the **User**.", false);

        swaggerDoc.ConfigureOperation(AuthMfaRecoveryCodeRegenerate, HttpMethod.Post, "post-auth-recovery-codes",
            "Generates recovery codes.", "Returns a new list of **Recovery codes**.");

        swaggerDoc.ConfigureOperation(AuthRefreshTokenEndpoint, HttpMethod.Post, "post-auth-refresh-token",
            "Refreshes an access token.", "Returns a refreshed **access token**.");

        swaggerDoc.ConfigureOperation(AuthRegisterEndpoint, HttpMethod.Post, "post-auth-register",
            "Registers a new user.", string.Empty, false);
    }
}