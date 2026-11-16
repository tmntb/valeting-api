using Common.Enums;

namespace Api.Context;

/// <summary>
/// Holds authenticated-user and client information for the current API request.
/// </summary>
public sealed class ValetingApiContext
{
    /// <summary>
    /// Authenticated user's identifier, or <see langword="null"/> for anonymous requests.
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// Authenticated user's display name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Authenticated user's email address.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Authenticated user's role.
    /// </summary>
    public RoleEnum? Role { get; set; }
    
    /// <summary>
    /// IP address resolved for the current request.
    /// </summary>
    public string? ClientIpAddress { get; set; }
}
