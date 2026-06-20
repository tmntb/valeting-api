using Service.Models.Auth.Payload;
using Service.Models.User;

namespace Service.Interfaces;

public interface IAuthService
{
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
}
