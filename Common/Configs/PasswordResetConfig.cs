namespace Common.Configs;

/// <summary>
/// Configures password reset links and token lifetime.
/// </summary>
public class PasswordResetConfig
{
    /// <summary>
    /// Public reset-page URL.
    /// </summary>
    public string BaseUrl { get; set; }

    /// <summary>
    /// Reset token lifetime in minutes.
    /// </summary>
    public int LifetimeMinutes { get; set; }
}