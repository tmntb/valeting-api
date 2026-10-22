using Service.Models.WebAuthn;

namespace Service.Interfaces;

public interface IWebAuthnChallengeStore
{
    /// <summary>
    /// Stores the state of a passkey ceremony.
    /// </summary>
    /// <param name="state">State of the ceremony to store.</param>
    void Store(PasskeyChallengeState state);

    /// <summary>
    /// Attempts to retrieve and consume the state of a passkey ceremony.
    /// </summary>
    /// <param name="ceremonyId">Unique identifier of the ceremony.</param>
    /// <param name="ceremony">Expected type of ceremony.</param>
    /// <param name="state">State of the ceremony if successfully consumed; otherwise, <see langword="null"/>.</param>
    /// <returns>
    /// <see langword="true"/> if the ceremony state was successfully retrieved and consumed; otherwise, <see langword="false"/>.
    /// </returns>
    bool TryConsume(Guid ceremonyId, PasskeyCeremony ceremony, out PasskeyChallengeState? state);
}
