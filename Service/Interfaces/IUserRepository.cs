using Service.Models.User;

namespace Service.Interfaces;

public interface IUserRepository
{
    /// <summary>
    /// Retrieves a user by their email/username from the database.
    /// </summary>
    /// <param name="email">The email or username of the user to retrieve.</param>
    /// <param name="cancellationToken">Token to cancel async call.</param>
    /// <returns>A task that returns a <see cref="UserDto"/> if found; otherwise, null.</returns>
    Task<UserDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a user by their unique identifier from the database.
    /// </summary>
    /// <param name="id">The unique identifier of the user to retrieve.</param>
    /// <param name="cancellationToken">Token to cancel async call.</param>
    /// <returns>A task that returns a <see cref="UserDto"/> if found; otherwise, null.</returns>
    Task<UserDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers a new user in the database.
    /// </summary>
    /// <param name="userDto">The user data to be registered.</param>
    /// <param name="cancellationToken">Token to cancel async call.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RegisterAsync(UserDto userDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing user's administrative settings in the database.
    /// </summary>
    /// <param name="userDto">The user data containing the updated administrative settings.</param>
    /// <param name="cancellationToken">Token to cancel async call.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateAdminSettingsAsync(UserDto userDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing user's email in the database.
    /// </summary>
    /// <param name="userDto">The user data containing the new email.</param>
    /// <param name="cancellationToken">Token to cancel async call.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateEmailAsync(UserDto userDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing user's last login timestamp in the database.
    /// </summary>
    /// <param name="userId">The unique identifier of the user to update.</param>
    /// <param name="cancellationToken">Token to cancel async call.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateLastLoginAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing user's mfa enable in the database.
    /// </summary>
    /// <param name="userDto">The user data containing the mfa enabled flag.</param>
    /// <param name="cancellationToken">Token to cancel async call.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateMfaEnableAsync(UserDto userDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing user's mfa secret in the database.
    /// </summary>
    /// <param name="userDto">The user data containing the mfa secret.</param>
    /// <param name="cancellationToken">Token to cancel async call.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateMfaSecretAsync(UserDto userDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing user's password in the database.
    /// </summary>
    /// <param name="userDto">The user data containing the new password hash.</param>
    /// <param name="cancellationToken">Token to cancel async call.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdatePasswordAsync(UserDto userDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing user's profile information in the database.
    /// </summary>
    /// <param name="userDto">The user data to be updated.</param>
    /// <param name="cancellationToken">Token to cancel async call.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateProfileAsync(UserDto userDto, CancellationToken cancellationToken = default);
}