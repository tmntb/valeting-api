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
    private const string AuthForgotPasswordEndpoint = "/auth/forgot-password";

    /// <summary>
    /// Endpoint for login.
    /// </summary>
    private const string AuthLoginEndpoint = "/auth/login";

    /// <summary>
    /// Endpoint for auth mfa regenerate recovery codes.
    /// </summary>
    private const string AuthMfaRecoveryCodeRegenerate = "/auth/recovery-codes";

    /// <summary>
    /// Endpoint to refresh user token
    /// </summary>
    private const string AuthRefreshTokenEndpoint = "/auth/refresh-token";

    /// <summary>
    /// Endpoint for user register.
    /// </summary>
    private const string AuthRegisterEndpoint = "/auth/register";

    private const string PasskeysEndpoint = "/auth/passkeys";
    private const string PasskeyByIdEndpoint = "/auth/passkeys/{id}";

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        swaggerDoc.Tags.Add(new OpenApiTag() { Name = "Auth", Description = "Auth operations" });

        swaggerDoc.ConfigureOperation(AuthForgotPasswordEndpoint, HttpMethod.Post,
            "post-auth-forgot-password", "Restores the user password.", string.Empty, false);
        
        swaggerDoc.ConfigureOperation(AuthLoginEndpoint, HttpMethod.Post,
            "post-auth-login", "Validates user credentials.", "Returns an access token for the **User**.", false);
        
        swaggerDoc.ConfigureOperation(AuthMfaRecoveryCodeRegenerate, HttpMethod.Post,
            "post-auth-recovery-codes", "Generates recovery codes.", "Returns a new list of **Recovery codes**.");
        
        swaggerDoc.ConfigureOperation(AuthRefreshTokenEndpoint, HttpMethod.Post,
            "post-auth-refresh-token", "Refreshes an access token.", "Returns a refreshed **access token**.");
        swaggerDoc.ConfigureOperation(AuthRegisterEndpoint, HttpMethod.Post,
            "post-auth-register", "Registers a new user.", string.Empty, false);

        
        swaggerDoc.ConfigureOperation(PasskeysEndpoint, HttpMethod.Get,
            "get-auth-passkeys", "List the current user's passkeys.",
            "Returns passkey names and lifecycle timestamps without credential material.");
        swaggerDoc.ConfigureOperation(PasskeyByIdEndpoint, HttpMethod.Put,
            "put-auth-passkeys-id", "Rename a passkey.",
            "Updates the friendly name of a passkey belonging to the authenticated user.");
        swaggerDoc.ConfigureOperation(PasskeyByIdEndpoint, HttpMethod.Delete,
            "delete-auth-passkeys-id", "Remove a passkey.",
            "Removes a passkey belonging to the authenticated user.");
    }
}
