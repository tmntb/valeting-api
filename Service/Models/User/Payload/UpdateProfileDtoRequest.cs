namespace Service.Models.User.Payload;

/// <summary>
/// Represents the data transfer object for updating a user's profile details.
/// </summary>
public class UpdateProfileDtoRequest
{
    /// <summary>
    /// Unique identifier of the user whose profile is being updated.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// First name of the user.
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Last name of the user.
    /// </summary>
    public string? LastName { get; set; }

    /// <summary> 
    /// Date of birth of the user.
    /// </summary>  
    public DateOnly? DateOfBirth { get; set; }

    /// <summary>
    /// Contact number of the user.
    /// </summary>
    public int? ContactNumber { get; set; }
}
