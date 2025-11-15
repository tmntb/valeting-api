using Service.Models.User.Payload;

namespace Service.Interfaces;

public interface IUserService
{
    /// <summary>
    /// Generates a JWT access token for the specified user.
    /// </summary>
    /// <param name="username">The username of the user for whom the token is generated.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a <see cref="GenerateTokenJWTDtoResponse"/> 
    /// with the token, its type, and expiration date.
    /// </returns>
    /// <exception cref="KeyNotFoundException">Thrown if the user with the given username does not exist.</exception>
    Task<GenerateTokenJWTDtoResponse> GenerateTokenJWTAsync(string username);

    /// <summary>
    /// Registers a new user with the provided username and password.
    /// </summary>
    /// <param name="registerDtoRequest">The registration information including username and password.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the username is already in use.</exception>
    Task RegisterAsync(RegisterDtoRequest registerDtoRequest);

    /// <summary>
    /// Validates the user's credentials by checking the username and password.
    /// </summary>
    /// <param name="validateLoginDtoRequest">The login information including username and password.</param>
    /// <returns>True if the username exists and the password matches; otherwise, false.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if no user is found with the given username.</exception>
    Task<bool> ValidateLoginAsync(ValidateLoginDtoRequest validateLoginSVRequest);

    /// <summary>
    /// Validates a JWT token and extracts the username claim.
    /// </summary>
    /// <param name="token">The JWT token to validate.</param>
    /// <returns>The username extracted from the token's claims.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown if the token is invalid, expired, or does not contain a username claim.</exception>
    string ValidateToken(string token);
}