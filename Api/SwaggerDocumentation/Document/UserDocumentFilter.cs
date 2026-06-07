using System.Diagnostics.CodeAnalysis;
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
    /// Endpoint for user login.
    /// </summary>
    public const string UserLoginEndpoint = "/users/login";

    /// <summary>
    /// Endpoint to refresh user token
    /// </summary>
    public const string UserRefreshTokenEndpoint = "/users/refreshToken";

    /// <summary>
    /// Endpoint for user reset password.
    /// </summary>
    public const string UserResetEndpoint = "/users/reset";

    /// <summary>
    /// Endpoint to update user admin settings.
    /// </summary>
    public const string UserAdminSettingsEndpoint = "/users/adminSettings";

    /// <summary>
    /// Endpoint to update user email.
    /// </summary>
    public const string UserEmailEndpoint = "/users/email";

    /// <summary>
    /// Endpoint to update user password.
    /// </summary>
    public const string UserPasswordEndpoint = "/users/password";

    /// <summary>
    /// Endpoint to update user profile.
    /// </summary>
    public const string UserProfileEndpoint = "/users/profile";

    /// <summary>
    /// Applies the filter to the given OpenAPI document.
    /// </summary>
    /// <param name="swaggerDoc">The OpenAPI document to modify.</param>
    /// <param name="context">The document filter context.</param>
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        swaggerDoc.Tags.Add(new OpenApiTag() { Name = "User", Description = "User operations" });

        var userLoginPaths = swaggerDoc.Paths.FirstOrDefault(x => x.Key == UserLoginEndpoint).Value;
        userLoginPaths.Operations.FirstOrDefault(x => x.Key == HttpMethod.Post).Value.OperationId = "post-login-user";
        userLoginPaths.Operations.FirstOrDefault(x => x.Key == HttpMethod.Post).Value.Summary = "Validates user credentials";
        userLoginPaths.Operations.FirstOrDefault(x => x.Key == HttpMethod.Post).Value.Description = "Returns an access token for the **User**";

        var userRefreshTokenPaths = swaggerDoc.Paths.FirstOrDefault(x => x.Key == UserRefreshTokenEndpoint).Value;
        userRefreshTokenPaths.Operations.FirstOrDefault(x => x.Key == HttpMethod.Post).Value.OperationId = "post-refresh-token-user";
        userRefreshTokenPaths.Operations.FirstOrDefault(x => x.Key == HttpMethod.Post).Value.Summary = "Refresh the token for valid user";
        userRefreshTokenPaths.Operations.FirstOrDefault(x => x.Key == HttpMethod.Post).Value.Description = "Returns a refreshed access token for the **User**";

        var userResetPaths = swaggerDoc.Paths.FirstOrDefault(x => x.Key == UserResetEndpoint).Value;
        userResetPaths.Operations.FirstOrDefault(x => x.Key == HttpMethod.Patch).Value.OperationId = "post-reset-password-user";
        userResetPaths.Operations.FirstOrDefault(x => x.Key == HttpMethod.Patch).Value.Summary = "Request a password reset";
        userResetPaths.Operations.FirstOrDefault(x => x.Key == HttpMethod.Patch).Value.Description = "Sends email with code to reset password for the **User**";

        var userAdminSettingsPaths = swaggerDoc.Paths.FirstOrDefault(x => x.Key == UserAdminSettingsEndpoint).Value;
        userAdminSettingsPaths.Operations.FirstOrDefault(x => x.Key == HttpMethod.Patch).Value.OperationId = "patch-admin-settings-user";
        userAdminSettingsPaths.Operations.FirstOrDefault(x => x.Key == HttpMethod.Patch).Value.Summary = "Update user admin settings";
        userAdminSettingsPaths.Operations.FirstOrDefault(x => x.Key == HttpMethod.Patch).Value.Description = "Updates the admin settings for the **User**";

        var userEmailPaths = swaggerDoc.Paths.FirstOrDefault(x => x.Key == UserEmailEndpoint).Value;
        userEmailPaths.Operations.FirstOrDefault(x => x.Key == HttpMethod.Patch).Value.OperationId = "patch-email-user";
        userEmailPaths.Operations.FirstOrDefault(x => x.Key == HttpMethod.Patch).Value.Summary = "Update user email";
        userEmailPaths.Operations.FirstOrDefault(x => x.Key == HttpMethod.Patch).Value.Description = "Updates the email for the **User**";

        var userPasswordPaths = swaggerDoc.Paths.FirstOrDefault(x => x.Key == UserPasswordEndpoint).Value;
        userPasswordPaths.Operations.FirstOrDefault(x => x.Key == HttpMethod.Patch).Value.OperationId = "patch-password-user";
        userPasswordPaths.Operations.FirstOrDefault(x => x.Key == HttpMethod.Patch).Value.Summary = "Update user password";
        userPasswordPaths.Operations.FirstOrDefault(x => x.Key == HttpMethod.Patch).Value.Description = "Updates the password for the **User**";

        var userProfilePaths = swaggerDoc.Paths.FirstOrDefault(x => x.Key == UserProfileEndpoint).Value;
        userProfilePaths.Operations.FirstOrDefault(x => x.Key == HttpMethod.Patch).Value.OperationId = "patch-profile-user";
        userProfilePaths.Operations.FirstOrDefault(x => x.Key == HttpMethod.Patch).Value.Summary = "Update user profile";
        userProfilePaths.Operations.FirstOrDefault(x => x.Key == HttpMethod.Patch).Value.Description = "Updates the profile for the **User**";
    }
}