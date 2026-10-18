using Api.Extensions;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api.SwaggerDocumentation.Document;

public class PasskeyDocumentFilter : IDocumentFilter
{
    /// <summary>
    /// Endpoint for register passkey options.
    /// </summary>
    private const string PasskeyRegisterOptions = "/passkey/register/options";

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        swaggerDoc.ConfigureOperation(
            PasskeyRegisterOptions, HttpMethod.Post,
            "post-auth-passkey-register-options", "Create passkey registration options.",
            "Returns browser WebAuthn registration options for the authenticated user."
        );
    }
}
