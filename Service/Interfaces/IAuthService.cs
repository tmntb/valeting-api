using Service.Models.Auth.Payload;
using Service.Models.User;

namespace Service.Interfaces;

public interface IAuthService
{
    /// <summary>
    /// Setups the user mfa secret and recovery codes
    /// </summary>
    /// <param name="email">The user emails</param>
    /// <returns>The mfa qr uri for user to setup and recovery codes.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the user mfa is already enabled.</exception>
    Task<MfaSetupDtoResponse> MfaSetupAsync(string email);

    /// <summary>
    /// Registers a new user with the provided username and password.
    /// </summary>
    /// <param name="userDto">The user registration information.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the username is already in use.</exception>
    Task RegisterAsync(UserDto userDto);
}
