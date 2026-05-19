using Service.Models.User;
using Service.Models.User.Payload;

namespace Service.Interfaces;

public interface IUserService
{
    /// <summary>
    /// Generates a JWT access token for the specified user.
    /// </summary>
    /// <param name="email">The email of the user for whom the token is generated.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a <see cref="GenerateTokenJWTDtoResponse"/> 
    /// with the token, its type, and expiration date.
    /// </returns>
    /// <exception cref="KeyNotFoundException">Thrown if the user with the given email does not exist.</exception>
    Task<GenerateTokenJWTDtoResponse> GenerateTokenJWTAsync(string email);

    /// <summary>
    /// Registers a new user with the provided username and password.
    /// </summary>
    /// <param name="userDto">The user registration information.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the username is already in use.</exception>
    Task RegisterAsync(UserDto userDto);

    /// <summary>
    /// Resets the password for the specified user.
    /// </summary>
    /// <param name="userDto">The user information.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if no user is found with the given email.</exception>
    Task ResetAsync(UserDto userDto);

    /// <summary>
    /// Updates the administrative settings for the specified user.
    /// </summary>
    /// <param name="userDto">The user information including new administrative settings.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if no user is found with the given email.</exception>
    Task UpdateAdminSettingsAsync(UserDto userDto);

    /// <summary>
    /// Updates the profile information for the specified user.
    /// </summary>
    /// <param name="userDto">The user information to be updated.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if no user is found with the given id.</exception>
    Task UpdateProfileAsync(UserDto userDto);

    /// <summary>
    /// Validates the user's credentials by checking the username and password.
    /// </summary>
    /// <param name="userDto">The user information.</param>
    /// <returns>True if the username exists and the password matches; otherwise, false.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if no user is found with the given username.</exception>
    Task ValidateLoginAsync(UserDto userDto);

    /// <summary>
    /// Validates a JWT token and extracts the username claim.
    /// </summary>
    /// <param name="token">The JWT token to validate.</param>
    /// <returns>The email extracted from the token's claims.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown if the token is invalid, expired, or does not contain a email claim.</exception>
    string ValidateToken(string token);
}