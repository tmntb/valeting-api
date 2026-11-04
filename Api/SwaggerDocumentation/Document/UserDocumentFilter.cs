using System.Diagnostics.CodeAnalysis;
using Api.Extensions;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api.SwaggerDocumentation.Document;

/// <summary>
/// Custom Swagger/OpenAPI document filter for the User endpoints.
/// </summary>
[ExcludeFromCodeCoverage]
public class UserDocumentFilter : IDocumentFilter
{
    /// <summary>
    /// Endpoint to update user admin settings.
    /// </summary>
    private const string UserAdminSettingsEndpoint = "/api/v1/users/admin-settings";

    /// <summary>
    /// Endpoint to update user email.
    /// </summary>
    private const string UserEmailEndpoint = "/api/v1/user/email";

    /// <summary>
    /// Endpoint to update user password.
    /// </summary>
    private const string UserPasswordEndpoint = "/api/v1/user/password";

    /// <summary>
    /// Endpoint to update user profile.
    /// </summary>
    private const string UserProfileEndpoint = "/api/v1/user/profile";

    /// <summary>
    /// Applies the filter to the given OpenAPI document.
    /// </summary>
    /// <param name="swaggerDoc">The OpenAPI document to modify.</param>
    /// <param name="context">The document filter context.</param>
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        swaggerDoc.Tags.Add(new OpenApiTag() { Name = "User", Description = "User operations" });
        
        swaggerDoc.ConfigureOperation(UserAdminSettingsEndpoint, HttpMethod.Patch, "patch-admin-settings-user",
            "Update user admin settings", "Updates the admin settings for the **User**");
        
        swaggerDoc.ConfigureOperation(UserEmailEndpoint, HttpMethod.Patch, "patch-email-user",
            "Update user email", "Updates the email for the **User**");
        
        swaggerDoc.ConfigureOperation(UserPasswordEndpoint, HttpMethod.Patch, "patch-password-user",
            "Update user password", "Updates the password for the **User**");
        
        swaggerDoc.ConfigureOperation(UserProfileEndpoint, HttpMethod.Patch, "patch-profile-user",
            "Update user profile", "Updates the profile for the **User**");
    }
}