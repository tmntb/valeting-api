using Service.Models.Auth.Payload;
using Service.Models.User;

namespace Service.Interfaces;

public interface IAuthService
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
    /// Enables mfa for a given user
    /// </summary>
    /// <param name="mfaCodeDtoRequest">The user mfa information.</param>
    /// <returns>A task representing the asynchronous operation</returns>
    /// <exception cref="InvalidOperationException">Thrown if the user mfa is already enabled.</exception>
    Task MfaEnableAsync(MfaCodeDtoRequest mfaCodeDtoRequest);

    /// <summary>
    /// Generate new recovery codes for a give user
    /// </summary>
    /// <param name="mfaCodeDtoRequest">The user mfa information.</param>
    /// <returns>The mfa qr uri for user to setup and recovery codes.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if the user doesn't exists.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the user mfa is already enabled.</exception>
    Task<List<string>> MfaRegenerateRecoveryCodesAsync(MfaCodeDtoRequest mfaCodeDtoRequest);

    /// <summary>
    /// Setups the user mfa secret and recovery codes
    /// </summary>
    /// <param name="userId">The user id</param>
    /// <returns>The mfa qr uri for user to setup and recovery codes.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if the user doesn't exists.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the user mfa is already enabled.</exception>
    Task<MfaSetupDtoResponse> MfaSetupAsync(Guid userId);

    /// <summary>
    /// Registers a new user with the provided username and password.
    /// </summary>
    /// <param name="userDto">The user registration information.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the username is already in use.</exception>
    Task RegisterAsync(UserDto userDto);

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
