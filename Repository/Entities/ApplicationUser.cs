namespace Repository.Entities;

/// <summary>
/// Represents an application user with credentials for authentication.
/// </summary>
public partial class ApplicationUser
{
    /// <summary>
    /// Unique identifier for the user.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Username of the user.
    /// </summary>
    public string Username { get; set; } = null!;

    /// <summary>
    /// Hashed password of the user.
    /// </summary>
    public string Password { get; set; } = null!;
}
