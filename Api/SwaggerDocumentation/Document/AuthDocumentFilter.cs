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
    /// Endpoint for user register.
    /// </summary>
    public const string AuthRegisterEndpoint = "/auth/register";

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var mfaEnablePaths = swaggerDoc.Paths.FirstOrDefault(x => x.Key.Contains(AuthMfaEnable)).Value;
        mfaEnablePaths.Operations.FirstOrDefault(x => x.Key == HttpMethod.Post).Value.OperationId = "post-auth-mfa-enable";
        mfaEnablePaths.Operations.FirstOrDefault(x => x.Key == HttpMethod.Post).Value.Summary = "Enables mfa";
        mfaEnablePaths.Operations.FirstOrDefault(x => x.Key == HttpMethod.Post).Value.Description = string.Empty;

        var mfaRecoveryCodeRegeneratePaths = swaggerDoc.Paths.FirstOrDefault(x => x.Key.Contains(AuthMfaRecoveryCodeRegenerate)).Value;
        mfaRecoveryCodeRegeneratePaths.Operations.FirstOrDefault(x => x.Key == HttpMethod.Post).Value.OperationId = "post-auth-mfa-recovery-codes-regenerates";
        mfaRecoveryCodeRegeneratePaths.Operations.FirstOrDefault(x => x.Key == HttpMethod.Post).Value.Summary = "Regenerates recovery codes";
        mfaRecoveryCodeRegeneratePaths.Operations.FirstOrDefault(x => x.Key == HttpMethod.Post).Value.Description = string.Empty;

        var mfaSetupPaths = swaggerDoc.Paths.FirstOrDefault(x => x.Key.Contains(AuthMfaSetup)).Value;
        mfaSetupPaths.Operations.FirstOrDefault(x => x.Key == HttpMethod.Post).Value.OperationId = "post-auth-mfa-setup";
        mfaSetupPaths.Operations.FirstOrDefault(x => x.Key == HttpMethod.Post).Value.Summary = "Setup mfa";
        mfaSetupPaths.Operations.FirstOrDefault(x => x.Key == HttpMethod.Post).Value.Description = string.Empty;

        var registerPaths = swaggerDoc.Paths.FirstOrDefault(x => x.Key.Contains(AuthRegisterEndpoint)).Value;
        registerPaths.Operations.FirstOrDefault(x => x.Key == HttpMethod.Post).Value.OperationId = "post-auth-register";
        registerPaths.Operations.FirstOrDefault(x => x.Key == HttpMethod.Post).Value.Summary = "Register a new user";
        registerPaths.Operations.FirstOrDefault(x => x.Key == HttpMethod.Post).Value.Description = string.Empty;
    }
}
