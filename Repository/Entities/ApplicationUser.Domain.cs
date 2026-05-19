namespace Repository.Entities;

public partial class ApplicationUser
{
    /// <summary>
    /// Updates the user's profile information, including first name, last name, date of birth, and contact number.
    /// </summary>
    /// <param name="firstName">The user's first name.</param>
    /// <param name="lastName">The user's last name.</param>
    /// <param name="dateOfBirth">The user's date of birth.</param>
    /// <param name="contactNumber">The user's contact number.</param>
    internal void UpdateProfile(string firstName, string lastName, DateOnly dateOfBirth, int contactNumber)
    {
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
        ContactNumber = contactNumber;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the user's password by setting a new password hash.
    /// </summary>
    /// <param name="passwordHash">The new password hash.</param>
    internal void UpdatePassword(string passwordHash)
    {
        PasswordHash = passwordHash;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the user's email address.
    /// </summary>
    /// <param name="email">The new email address.</param>
    internal void UpdateEmail(string email)
    {
        Email = email;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the user's administrative settings, including role and active status.
    /// </summary>
    /// <param name="roleId">The unique identifier of the role to be assigned to the user.</param>
    /// <param name="isActive">A boolean value indicating whether the user is active or not.</param>
    internal void UpdateAdminSettings(Guid roleId, bool isActive)
    {
        RoleId = roleId;
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;
    }


    /// <summary>
    /// Updates the user's last login timestamp to the current UTC time.
    /// </summary>
    internal void UpdateLastLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
