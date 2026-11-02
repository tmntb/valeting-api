using Service.Models.User;
using Service.Models.WebAuthn;
using Service.Models.WebAuthn.Payload;

namespace Service.Interfaces;

public interface IWebAuthnService
{
    /// <summary>
    /// Creates browser-compatible assertion options for optional-email or discoverable passkey sign-in.
    /// </summary>
    /// <param name="userDto">User info</param>
    /// <param name="cancellationToken">Cancels the operation.</param>
    /// <returns>The WebAuthn public-key options and single-use ceremony identifier.</returns>
    Task<WebAuthnAuthenticationOptionsDtoResponse> CreateAuthenticationOptionsAsync(UserDto userDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates a WebAuthn assertion and resolves the authenticated user.
    /// </summary>
    /// <param name="webAuthnAuthenticationVerifyRequest">The ceremony identifier and raw browser assertion response.</param>
    /// <param name="cancellationToken">Cancels the operation.</param>
    Task VerifyAuthenticationAsync(WebAuthnAuthenticationVerifyRequest webAuthnAuthenticationVerifyRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates browser-compatible credential-creation options for a user.
    /// </summary>
    /// <param name="userId">The authenticated user identifier.</param>
    /// <param name="cancellationToken">Cancels the operation.</param>
    /// <returns>The WebAuthn creation options and single-use ceremony identifier.</returns>
    Task<WebAuthnRegistrationOptionsDtoResponse> CreateRegistrationOptionsAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates and stores a WebAuthn attestation for a user.
    /// </summary>
    /// <param name="webAuthnRegistrationVerifyDtoRequest">The user identifier, ceremony identifier, attestation response, and optional friendly name.</param>
    /// <param name="cancellationToken">Cancels the operation.</param>
    Task VerifyRegisterAsync(WebAuthnRegistrationVerifyDtoRequest webAuthnRegistrationVerifyDtoRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists a user's webauthn records.
    /// </summary>
    /// <param name="userId">The authenticated user identifier.</param>
    /// <param name="cancellationToken">Cancels the operation.</param>
    /// <returns>The user's stored webauthn's.</returns>
    Task<IEnumerable<WebAuthnDto>> GetUserWebAuthnAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a webauthn owned by a user.
    /// </summary>
    /// <param name="webauthnId">The webauthn identifier.</param>
    /// <param name="cancellationToken">Cancels the operation.</param>
    Task RemoveWebAuthnAsync(Guid webauthnId, CancellationToken cancellationToken = default);
}