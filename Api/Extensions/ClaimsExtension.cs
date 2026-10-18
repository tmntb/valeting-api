using System.Security.Claims;

namespace Api.Extensions;

public static class UserClaimsHelper
{
    /// <summary>
    /// Gets the current authenticated user's ID from the claims principal.
    /// </summary>
    /// <param name="user">The claims principal representing the authenticated user.</param>
    /// <returns>The user's ID.</returns>
    internal static Guid GetCurrentUserId(this ClaimsPrincipal user) => Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value);
}
