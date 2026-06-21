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
    /// Endpoint for login.
    /// </summary>
    public const string AuthLoginEndpoint = "/auth/login";

    /// <summary>
    /// Endpoint for auth mfa enable.
    /// </summary>
    public const string AuthMfaEnable = "/auth/mfa/enable";

    /// <summary>
    /// Endpoint for auth mfa regenerate recovery codes.
    /// </summary>
    public const string AuthMfaRecoveryCodeRegenerate = "/auth/mfa/recovery-codes/regenerate";

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

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        swaggerDoc.Tags.Add(new OpenApiTag() { Name = "Auth", Description = "Auth operations" });

        var userLoginPaths = swaggerDoc.Paths.FirstOrDefault(x => x.Key.Contains(AuthLoginEndpoint)).Value;
        userLoginPaths.Operations.FirstOrDefault(x => x.Key == HttpMethod.Post).Value.OperationId = "post-auth-login";
        userLoginPaths.Operations.FirstOrDefault(x => x.Key == HttpMethod.Post).Value.Summary = "Validates user credentials";
        userLoginPaths.Operations.FirstOrDefault(x => x.Key == HttpMethod.Post).Value.Description = "Returns an access token for the **User**";

        var mfaEnablePaths = swaggerDoc.Paths.FirstOrDefault(x => x.Key.Contains(AuthMfaEnable)).Value;
        mfaEnablePaths.Operations.FirstOrDefault(x => x.Key == HttpMethod.Post).Value.OperationId = "post-auth-mfa-enable";
        mfaEnablePaths.Operations.FirstOrDefault(x => x.Key == HttpMethod.Post).Value.Summary = "Enables mfa";
        mfaEnablePaths.Operations.FirstOrDefault(x => x.Key == HttpMethod.Post).Value.Description = string.Empty;

        var mfaRecoveryCodeRegeneratePaths = swaggerDoc.Paths.FirstOrDefault(x => x.Key.Contains(AuthMfaRecoveryCodeRegenerate)).Value;
        mfaRecoveryCodeRegeneratePaths.Operations.FirstOrDefault(x => x.Key == HttpMethod.Post).Value.OperationId = "post-auth-mfa-recovery-codes-regenerates";
        mfaRecoveryCodeRegeneratePaths.Operations.FirstOrDefault(x => x.Key == HttpMethod.Post).Value.Summary = "Regenerates recovery codes";
        mfaRecoveryCodeRegeneratePaths.Operations.FirstOrDefault(x => x.Key == HttpMethod.Post).Value.Description = "Returns a new list of **Recovery codes**";

        var mfaSetupPaths = swaggerDoc.Paths.FirstOrDefault(x => x.Key.Contains(AuthMfaSetup)).Value;
        mfaSetupPaths.Operations.FirstOrDefault(x => x.Key == HttpMethod.Post).Value.OperationId = "post-auth-mfa-setup";
        mfaSetupPaths.Operations.FirstOrDefault(x => x.Key == HttpMethod.Post).Value.Summary = "Setup mfa";
        mfaSetupPaths.Operations.FirstOrDefault(x => x.Key == HttpMethod.Post).Value.Description = "Returns the **mfa** info required for the setup.";

        var userRefreshTokenPaths = swaggerDoc.Paths.FirstOrDefault(x => x.Key.Contains(AuthRefreshTokenEndpoint)).Value;
        userRefreshTokenPaths.Operations.FirstOrDefault(x => x.Key == HttpMethod.Post).Value.OperationId = "post-auth-refresh-token";
        userRefreshTokenPaths.Operations.FirstOrDefault(x => x.Key == HttpMethod.Post).Value.Summary = "Refresh the token for valid user";
        userRefreshTokenPaths.Operations.FirstOrDefault(x => x.Key == HttpMethod.Post).Value.Description = "Returns a refreshed **access token**";

        var registerPaths = swaggerDoc.Paths.FirstOrDefault(x => x.Key.Contains(AuthRegisterEndpoint)).Value;
        registerPaths.Operations.FirstOrDefault(x => x.Key == HttpMethod.Post).Value.OperationId = "post-auth-register";
        registerPaths.Operations.FirstOrDefault(x => x.Key == HttpMethod.Post).Value.Summary = "Register a new user";
        registerPaths.Operations.FirstOrDefault(x => x.Key == HttpMethod.Post).Value.Description = string.Empty;
    }
}
