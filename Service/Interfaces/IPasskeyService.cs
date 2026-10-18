using Service.Models.Auth;
using Service.Models.Passkey;
using Service.Models.Passkey.Payload;

namespace Service.Interfaces;

public interface IPasskeyService
{
    /// <summary>Creates browser-compatible assertion options for optional-email or discoverable passkey sign-in.</summary>
    /// <param name="email">Optional email used to restrict the assertion to a user's registered passkeys.</param>
    /// <param name="cancellationToken">Cancels the operation.</param>
    /// <returns>The WebAuthn public-key options and single-use ceremony identifier.</returns>
    Task<PasskeyAuthenticationOptionsResponse> CreateAuthenticationOptionsAsync(string? email = null, CancellationToken cancellationToken = default);

    /// <summary>Validates a WebAuthn assertion and resolves the authenticated user.</summary>
    /// <param name="request">The ceremony identifier and raw browser assertion response.</param>
    /// <param name="cancellationToken">Cancels the operation.</param>
    /// <returns>The verified user identity for token issuance.</returns>
    Task<PasskeyAuthenticationResult> VerifyAuthenticationAsync(PasskeyAuthenticationVerifyRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates browser-compatible credential-creation options for a user.
    /// </summary>
    /// <param name="userId">The authenticated user identifier.</param>
    /// <param name="cancellationToken">Cancels the operation.</param>
    /// <returns>The WebAuthn creation options and single-use ceremony identifier.</returns>
    Task<PasskeyRegistrationOptionsDtoResponse> CreateRegistrationOptionsAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates and stores a WebAuthn attestation for a user.
    /// </summary>
    /// <param name="passkeyRegistrationsVerifyDtoRequest">The user identifier, ceremony identifier, attestation response, and optional friendly name.</param>
    /// <param name="cancellationToken">Cancels the operation.</param>
    Task RegisterCredentialAsync(PasskeyRegistrationVerifyDtoRequest passkeyRegistrationVerifyDtoRequest, CancellationToken cancellationToken = default);

    /// <summary>Lists a user's passkey records for API-safe mapping.</summary>
    /// <param name="userId">The authenticated user identifier.</param>
    /// <param name="cancellationToken">Cancels the operation.</param>
    /// <returns>The user's stored passkeys.</returns>
    Task<IEnumerable<PasskeyDto>> GetUserPasskeysAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>Renames a passkey owned by a user.</summary>
    /// <param name="userId">The authenticated user identifier.</param>
    /// <param name="passkeyId">The API passkey identifier.</param>
    /// <param name="friendlyName">The new friendly name.</param>
    /// <param name="cancellationToken">Cancels the operation.</param>
    Task RenamePasskeyAsync(Guid userId, Guid passkeyId, string friendlyName, CancellationToken cancellationToken = default);

    /// <summary>Deletes a passkey owned by a user.</summary>
    /// <param name="userId">The authenticated user identifier.</param>
    /// <param name="passkeyId">The API passkey identifier.</param>
    /// <param name="cancellationToken">Cancels the operation.</param>
    Task RemovePasskeyAsync(Guid userId, Guid passkeyId, CancellationToken cancellationToken = default);
}
