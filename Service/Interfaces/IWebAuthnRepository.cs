using Service.Models.Auth;
using Service.Models.WebAuthn;

namespace Service.Interfaces;

public interface IWebAuthnRepository
{
    /// <summary>
    /// Stores a newly registered webauthn for a user.
    /// </summary>
    /// <param name="webAuthnDto">The webauthn data to persist.</param>
    /// <param name="cancellationToken">Cancels the operation.</param>
    Task CreateAsync(WebAuthnDto webAuthnDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all webauthns registered to a specific user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="cancellationToken">Cancels the operation.</param>
    /// <returns>The collection of webauthn's associated with the user.</returns>
    Task<IEnumerable<WebAuthnDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a webauthn by its WebAuthn credential identifier.
    /// </summary>
    /// <param name="credentialId">The unique WebAuthn credential identifier.</param>
    /// <param name="cancellationToken">Cancels the operation.</param>
    /// <returns>The matching webauthn, if one exists.</returns>
    Task<WebAuthnDto?> GetByCredentialIdAsync(string credentialId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the metadata of an existing webauthn.
    /// </summary>
    /// <param name="webAuthnDto">The updated webauthn record.</param>
    /// <param name="cancellationToken">Cancels the operation.</param>
    Task UpdateAsync(WebAuthnDto webAuthnDto, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates webauthn signal counter.
    /// </summary>
    /// <param name="webAuthnDto">The webauthn dto info.</param>
    /// <param name="cancellationToken">Cancels the operation.</param>
    Task UpdateSignalCounter(WebAuthnDto webAuthnDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a webauthn.
    /// </summary>
    /// <param name="id">The webauthn identifier.</param>
    /// <param name="cancellationToken">Cancels the operation.</param>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
