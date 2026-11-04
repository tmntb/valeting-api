namespace Service.Models.User.Payload;

public class UpdateAdminSettingsDtoRequest
{
    /// <summary>
    /// The unique identifier of the administrator performing the update.
    /// </summary>
    public Guid AdminId { get; set; }

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
