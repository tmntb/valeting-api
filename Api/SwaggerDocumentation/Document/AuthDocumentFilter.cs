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
    /// Endpoint for completing password reset.
    /// </summary>
    private const string AuthResetPasswordEndpoint = "/api/v1/auth/reset-password";

    /// <summary>
    /// Endpoint for login.
    /// </summary>
    private const string AuthLoginEndpoint = "/api/v1/auth/login";

    /// <summary>
    /// Endpoint for auth mfa regenerate recovery codes.
    /// </summary>
    private const string AuthMfaRecoveryCodeRegenerate = "/api/v1/auth/recovery-codes";

    /// <summary>
    /// Endpoint for user register.
    /// </summary>
    private const string AuthRegisterEndpoint = "/api/v1/auth/register";

    /// <summary>
    /// Adds the Auth tag and custom descriptions to authentication operations.
    /// </summary>
    /// <param name="swaggerDoc">The generated OpenAPI document to update.</param>
    /// <param name="context">Metadata about the document generation process.</param>
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        swaggerDoc.Tags.Add(new OpenApiTag() { Name = "Auth", Description = "Auth operations" });

        swaggerDoc.ConfigureOperation(AuthForgotPasswordEndpoint, HttpMethod.Post, "post-auth-forgot-password", 
            "Initiates password recovery.", "Sends a password reset link to the **User** when the account exists.", false);

        swaggerDoc.ConfigureOperation(AuthResetPasswordEndpoint, HttpMethod.Post, "post-auth-reset-password",
            "Completes password recovery.", "Updates the password using a valid one-time **reset token**.", false);

        swaggerDoc.ConfigureOperation(AuthLoginEndpoint, HttpMethod.Post, "post-auth-login",
            "Validates user credentials.", "Returns an access token for the **User**.", false);

        swaggerDoc.ConfigureOperation(AuthMfaRecoveryCodeRegenerate, HttpMethod.Post, "post-auth-recovery-codes",
            "Generates recovery codes.", "Returns a new list of **Recovery codes**.");

        swaggerDoc.ConfigureOperation(AuthRegisterEndpoint, HttpMethod.Post, "post-auth-register",
            "Registers a new user.", string.Empty, false);
    }
}