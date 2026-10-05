using Service.Models.User;
using Service.Models.User.Payload;

namespace Service.Interfaces;

public interface IUserService
{
    /// <summary>
    /// Updates the administrative settings for the specified user.
    /// </summary>
    /// <param name="updateAdminSettingsDtoRequest">The request containing the updated administrative settings.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if no user is found with the given email.</exception>
    Task UpdateAdminSettingsAsync(UpdateAdminSettingsDtoRequest updateAdminSettingsDtoRequest);

    /// <summary>
    /// Updates the email for the specified user.
    /// </summary>
    /// <param name="userDto">The user information to be updated.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if no user is found with the given id.</exception>
    Task UpdateEmailAsync(UserDto userDto);

    /// <summary>
    /// Updates the password for the specified user.
    /// </summary>
    /// <param name="userDto">The user information to be updated, including the new password.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if no user is found with the given id.</exception>
    Task UpdatePasswordAsync(UserDto userDto);

    /// <summary>
    /// Updates the profile information for the specified user.
    /// </summary>
    /// <param name="updateProfileDtoRequest">The request containing the updated profile information.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if no user is found with the given id.</exception>
    Task UpdateProfileAsync(UpdateProfileDtoRequest updateProfileDtoRequest);
}