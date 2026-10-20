using Service.Models.Auth;

namespace Service.Interfaces;

public interface IWebAuthnRepository
{
    /// <summary>
    /// Stores a newly registered passkey for a user.
    /// </summary>
    /// <param name="webAuthnDto">The passkey data to persist.</param>
    /// <param name="cancellationToken">Cancels the operation.</param>
    Task CreateAsync(WebAuthnDto webAuthnDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all passkeys registered to a specific user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="cancellationToken">Cancels the operation.</param>
    /// <returns>The collection of passkeys associated with the user.</returns>
    Task<IEnumerable<WebAuthnDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a passkey by its WebAuthn credential identifier.
    /// </summary>
    /// <param name="credentialId">The unique WebAuthn credential identifier.</param>
    /// <param name="cancellationToken">Cancels the operation.</param>
    /// <returns>The matching passkey, if one exists.</returns>
    Task<WebAuthnDto?> GetByCredentialIdAsync(string credentialId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the metadata of an existing passkey.
    /// </summary>
    /// <param name="webAuthnDto">The updated passkey record.</param>
    /// <param name="cancellationToken">Cancels the operation.</param>
    Task UpdateAsync(WebAuthnDto webAuthnDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a passkey owned by the specified user.
    /// </summary>
    /// <param name="id">The passkey identifier.</param>
    /// <param name="userId">The owning user identifier.</param>
    /// <param name="cancellationToken">Cancels the operation.</param>
    Task DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
}
