using Api.Extensions;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api.SwaggerDocumentation.Document;

public class SecurityDocumentFilter : IDocumentFilter
{
    /// <summary>
    /// Endpoint for auth mfa enable.
    /// </summary>
    private const string MfaEnable = "/api/v1/security/mfa/verify";

    /// <summary>
    /// Endpoint for auth mfa setup.
    /// </summary>
    private const string MfaSetup = "/api/v1/security/mfa/setup";

    /// <summary>
    /// Endpoint for authenticate webauthn options.
    /// </summary>
    private const string WebAuthnAuthenticateOptions = "/api/v1/security/webauthn/authenticate/options";

    /// <summary>
    /// Endpoint for verify authentication webauthn options.
    /// </summary>
    private const string WebAuthnAuthenticateVerify = "/api/v1/security/webauthn/authenticate/verify";

    /// <summary>
    /// Endpoint for register webauthn options.
    /// </summary>
    private const string WebAuthnRegisterOptions = "/api/v1/security/webauthn/register/options";

    /// <summary>
    /// Endpoint for verify register webauthn.
    /// </summary>
    private const string WebAuthnRegisterVerify = "/api/v1/security/webauthn/register/verify";

    /// <summary>
    /// Endpoint to retrieve user webauthn's.
    /// </summary>
    private const string WebAuthnEndpoint = "/api/v1/security/webauthns";

    /// <summary>
    /// Endpoint to delete a webauthn.
    /// </summary>
    private const string WebAuthnDeleteEndpoint = "/api/v1/security/webauthn/{id}";

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        swaggerDoc.ConfigureOperation(
            MfaEnable, HttpMethod.Post,
            "post-mfa-verify", "Verifies an MFA code.", string.Empty);

        swaggerDoc.ConfigureOperation(
            MfaSetup, HttpMethod.Post,
            "post-mfa-setup", "Sets up MFA.",
            "Returns the MFA information required for setup.");

        swaggerDoc.ConfigureOperation(
            WebAuthnAuthenticateOptions, HttpMethod.Post,
            "post-webauthn-authenticate-options", "Create webauthn sign-in options.",
            "Returns browser WebAuthn assertion options and a short-lived, single-use ceremony ID.", false);

        swaggerDoc.ConfigureOperation(
            WebAuthnAuthenticateVerify, HttpMethod.Post,
            "post-webauthn-authenticate-verify", "Verify webauthn sign-in.",
            "Validates the WebAuthn assertion.", false);

        swaggerDoc.ConfigureOperation(
            WebAuthnRegisterOptions, HttpMethod.Post,
            "post-webauthn-register-options", "Create webauthn registration options.",
            "Returns browser WebAuthn registration options for the authenticated user."
        );

        swaggerDoc.ConfigureOperation(
            WebAuthnRegisterVerify, HttpMethod.Post,
            "post-webauthn-register-verify", "Verify webauthn registration.",
            "Validates the attestation before storing the new credential.");


        swaggerDoc.ConfigureOperation(
            WebAuthnEndpoint, HttpMethod.Get,
            "get-webauthn", "List the current user's webauthn.",
            "Returns webauthn names and lifecycle timestamps without credential material.");

        swaggerDoc.ConfigureOperation(
            WebAuthnDeleteEndpoint, HttpMethod.Delete,
            "delete-webauthn-id", "Remove a webauthn.",
            "Removes a webauthn.");
    }
}