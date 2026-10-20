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
    public const string AuthForgotPasswordEndpoint = "/auth/forgot-password";

    /// <summary>
    /// Endpoint for login.
    /// </summary>
    public const string AuthLoginEndpoint = "/auth/login";

    /// <summary>
    /// Endpoint for auth mfa enable.
    /// </summary>
    public const string AuthMfaEnable = "/auth/mfa/verify";

    /// <summary>
    /// Endpoint for auth mfa regenerate recovery codes.
    /// </summary>
    public const string AuthMfaRecoveryCodeRegenerate = "/auth/recovery-codes";

    /// <summary>
    /// Endpoint for auth mfa setup.
    /// </summary>
    public const string AuthMfaSetup = "/auth/mfa/setup";

    /// <summary>
    /// Endpoint to refresh user token
    /// </summary>
    public const string AuthRefreshTokenEndpoint = "/auth/refresh-token";

    /// <summary>
    /// Endpoint for user register.
    /// </summary>
    public const string AuthRegisterEndpoint = "/auth/register";

    private const string PasskeyAuthenticateOptions = "/auth/passkey/authenticate/options";
    private const string PasskeyAuthenticateVerify = "/auth/passkey/authenticate/verify";

    private const string PasskeysEndpoint = "/auth/passkeys";
    private const string PasskeyByIdEndpoint = "/auth/passkeys/{id}";

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        swaggerDoc.Tags.Add(new OpenApiTag() { Name = "Auth", Description = "Auth operations" });

        swaggerDoc.ConfigureOperation(AuthForgotPasswordEndpoint, HttpMethod.Post,
            "post-auth-forgot-password", "Restores the user password.", string.Empty, false);
        swaggerDoc.ConfigureOperation(AuthLoginEndpoint, HttpMethod.Post,
            "post-auth-login", "Validates user credentials.", "Returns an access token for the **User**.", false);
        swaggerDoc.ConfigureOperation(AuthMfaEnable, HttpMethod.Post,
            "post-auth-mfa-verify", "Verifies an MFA code.", string.Empty);
        swaggerDoc.ConfigureOperation(AuthMfaRecoveryCodeRegenerate, HttpMethod.Post,
            "post-auth-recovery-codes", "Generates recovery codes.", "Returns a new list of **Recovery codes**.");
        swaggerDoc.ConfigureOperation(AuthMfaSetup, HttpMethod.Post,
            "post-auth-mfa-setup", "Sets up MFA.", "Returns the MFA information required for setup.");
        swaggerDoc.ConfigureOperation(AuthRefreshTokenEndpoint, HttpMethod.Post,
            "post-auth-refresh-token", "Refreshes an access token.", "Returns a refreshed **access token**.");
        swaggerDoc.ConfigureOperation(AuthRegisterEndpoint, HttpMethod.Post,
            "post-auth-register", "Registers a new user.", string.Empty, false);

        swaggerDoc.ConfigureOperation(PasskeyAuthenticateOptions, HttpMethod.Post,
            "post-auth-passkey-authenticate-options", "Create passkey sign-in options.",
            "Returns browser WebAuthn assertion options and a short-lived, single-use ceremony ID.", false);
        swaggerDoc.ConfigureOperation(PasskeyAuthenticateVerify, HttpMethod.Post,
            "post-auth-passkey-authenticate-verify", "Verify passkey sign-in.",
            "Validates the WebAuthn assertion before issuing an access token.", false);

        
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
