using Service.Models.Auth.Payload;
using Service.Models.Mfa.Payload;

namespace Service.Interfaces;

public interface IMfaService
{
    /// <summary>
    /// Setups the user mfa secret
    /// </summary>
    /// <param name="userId">The user id</param>
    /// <param name="cancellationToken">The cancellation token that cancel the operation.</param>
    /// <returns>The mfa qr uri for user to setup.</returns>
    Task<MfaSetupDtoResponse> MfaSetupAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies mfa for a given user and enables it if the code is valid.
    /// </summary>
    /// <param name="mfaCodeDtoRequest">The user mfa information.</param>
    /// <param name="cancellationToken">The cancellation token that cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task MfaVerifyAsync(MfaCodeDtoRequest mfaCodeDtoRequest, CancellationToken cancellationToken = default);
}
