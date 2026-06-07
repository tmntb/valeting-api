using Service.Models.Auth.Payload;
using Service.Models.User;

namespace Service.Interfaces;

public interface IAuthService
{
    /// <summary>
    /// Registers a new user with the provided username and password.
    /// </summary>
    /// <param name="userDto">The user registration information.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the username is already in use.</exception>
    Task RegisterAsync(UserDto userDto);

    Task<MfaSetupDtoResponse> MfaSetupAsync();
}
