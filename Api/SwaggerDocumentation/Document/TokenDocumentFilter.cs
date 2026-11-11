using System.Diagnostics.CodeAnalysis;
using Api.Extensions;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api.SwaggerDocumentation.Document;

/// <summary>
/// Custom Swagger/OpenAPI document filter for the Token endpoints.
/// </summary>
[ExcludeFromCodeCoverage]
public class TokenDocumentFilter : IDocumentFilter
{
    /// <summary>
    /// Endpoint to generate access token
    /// </summary>
    private const string GenerateTokenEndpoint = "/api/v1/token/generate";
    
    /// <summary>
    /// Endpoint to refresh access token
    /// </summary>
    private const string RefreshTokenEndpoint = "/api/v1/token/refresh";

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        swaggerDoc.ConfigureOperation(GenerateTokenEndpoint, HttpMethod.Post, "post-token-generate",
            "Generates an access token.", "Returns a newly **access token**.");
        
        swaggerDoc.ConfigureOperation(RefreshTokenEndpoint, HttpMethod.Post, "post-token-refresh",
            "Refreshes an access token.", "Returns a refreshed **access token**.");
    }
}