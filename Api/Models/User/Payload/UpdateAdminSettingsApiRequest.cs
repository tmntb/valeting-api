namespace Api.Models.User.Payload;

/// <summary>
/// Represents the request payload for updating a user's administrative settings.
/// </summary>
public class UpdateAdminSettingsApiRequest
{   
    /// <summary>
    /// The unique identifier of the user whose administrative settings are to be updated.
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// Indicates whether the user is active or not.
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// Role identifier to assign to the user.
    /// </summary>
    public Guid? RoleId { get; set; }
}
