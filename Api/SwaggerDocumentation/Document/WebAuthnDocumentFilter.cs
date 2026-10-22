using Api.Extensions;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api.SwaggerDocumentation.Document;

public class WebAuthnDocumentFilter : IDocumentFilter
{
    /// <summary>
    /// Endpoint for auth mfa enable.
    /// </summary>
    private const string MfaEnable = "/session/mfa/verify";
    
    /// <summary>
    /// Endpoint for auth mfa setup.
    /// </summary>
    private const string MfaSetup = "/session/mfa/setup";
    
    /// <summary>
    /// Endpoint for authenticate webauthn options.
    /// </summary>
    private const string WebAuthnAuthenticateOptions = "/session/webauthn/authenticate/options";
    
    /// <summary>
    /// Endpoint for verify authentication webauthn options.
    /// </summary>
    private const string WebAuthnAuthenticateVerify = "/session/webauthn/authenticate/verify";
    
    /// <summary>
    /// Endpoint for register webauthn options.
    /// </summary>
    private const string WebAuthnRegisterOptions = "/session/webauthn/register/options";

    /// <summary>
    /// Endpoint for verify register webauthn.
    /// </summary>
    private const string WebAuthnRegisterVerify = "/session/webauthn/register/verify";

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        swaggerDoc.ConfigureOperation(
            MfaEnable, HttpMethod.Post,
            "post-mfa-verify", "Verifies an MFA code.", string.Empty);
        
        swaggerDoc.ConfigureOperation(
            MfaSetup, HttpMethod.Post,
            "post-mfa-setup", "Sets up MFA.", 
            "Returns the MFA information required for setup.");
        
        swaggerDoc.ConfigureOperation(WebAuthnAuthenticateOptions, HttpMethod.Post,
            "post-webauthn-authenticate-options", "Create webauthn sign-in options.",
            "Returns browser WebAuthn assertion options and a short-lived, single-use ceremony ID.");
        
        swaggerDoc.ConfigureOperation(WebAuthnAuthenticateVerify, HttpMethod.Post,
            "post-webauthn-authenticate-verify", "Verify webauthn sign-in.",
            "Validates the WebAuthn assertion.");
        
        swaggerDoc.ConfigureOperation(
            WebAuthnRegisterOptions, HttpMethod.Post,
            "post-webauthn-register-options", "Create webauthn registration options.",
            "Returns browser WebAuthn registration options for the authenticated user."
        );

        swaggerDoc.ConfigureOperation(
            WebAuthnRegisterVerify, HttpMethod.Post,
            "post-webauthn-register-verify", "Verify webauthn registration.",
            "Validates the attestation before storing the new credential.");
    }
}
