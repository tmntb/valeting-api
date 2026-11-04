using Service.Models.User;
using Service.Models.User.Payload;

namespace Service.Interfaces;

public interface IUserService
{
    /// <summary>
    /// Updates the administrative settings for the specified user.
    /// </summary>
    /// <param name="updateAdminSettingsDtoRequest">The request containing the updated administrative settings.</param>
    /// <param name="cancellationToken">The cancellation token that cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateAdminSettingsAsync(UpdateAdminSettingsDtoRequest updateAdminSettingsDtoRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the email for the specified user.
    /// </summary>
    /// <param name="userDto">The user information to be updated.</param>
    /// <param name="cancellationToken">The cancellation token that cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateEmailAsync(UserDto userDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the password for the specified user.
    /// </summary>
    /// <param name="userDto">The user information to be updated, including the new password.</param>
    /// <param name="cancellationToken">The cancellation token that cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdatePasswordAsync(UserDto userDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the profile information for the specified user.
    /// </summary>
    /// <param name="updateProfileDtoRequest">The request containing the updated profile information.</param>
    /// <param name="cancellationToken">The cancellation token that cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateProfileAsync(UpdateProfileDtoRequest updateProfileDtoRequest, CancellationToken cancellationToken = default);
}