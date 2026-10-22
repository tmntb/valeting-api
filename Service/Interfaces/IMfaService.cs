using Service.Models.Auth.Payload;
using Service.Models.Mfa.Payload;

namespace Service.Interfaces;

public interface IMfaService
{
    /// <summary>
    /// Setups the user mfa secret
    /// </summary>
    /// <param name="userId">The user id</param>
    /// <returns>The mfa qr uri for user to setup.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if the user doesn't exists.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the user mfa is already enabled.</exception>
    Task<MfaSetupDtoResponse> MfaSetupAsync(Guid userId);

    /// <summary>
    /// Verifies mfa for a given user and enables it if the code is valid.
    /// </summary>
    /// <param name="mfaCodeDtoRequest">The user mfa information.</param>
    /// <returns>A task representing the asynchronous operation</returns>
    /// <exception cref="InvalidOperationException">Thrown if the user mfa is already enabled.</exception>
    Task MfaVerifyAsync(MfaCodeDtoRequest mfaCodeDtoRequest);
}
