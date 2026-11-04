namespace Api.Models.User.Payload;

/// <summary>
/// Represents the payload for updating a user's email address.
/// </summary>
public class UpdateEmailApiRequest
{
    /// <summary>
    /// New email address for the user.
    /// </summary>
    public string Email { get; set; }
}
