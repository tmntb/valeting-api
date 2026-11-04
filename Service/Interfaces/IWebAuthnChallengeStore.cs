using Service.Models.WebAuthn;

namespace Service.Interfaces;

public interface IWebAuthnChallengeStore
{
    /// <summary>
    /// Stores the state of a webauthn ceremony.
    /// </summary>
    /// <param name="webAuthnChallengeState">State of the ceremony to store.</param>
    void Store(WebAuthnChallengeState webAuthnChallengeState);

    /// <summary>
    /// Attempts to retrieve and consume the state of a webauthn ceremony.
    /// </summary>
    /// <param name="ceremonyId">Unique identifier of the ceremony.</param>
    /// <param name="webAuthnCeremony">Expected type of ceremony.</param>
    /// <param name="webAuthnChallengeState">State of the ceremony if successfully consumed; otherwise, <see langword="null"/>.</param>
    /// <returns>
    /// <see langword="true"/> if the ceremony state was successfully retrieved and consumed; otherwise, <see langword="false"/>.
    /// </returns>
    bool TryConsume(Guid ceremonyId, WebAuthnCeremony webAuthnCeremony, out WebAuthnChallengeState? webAuthnChallengeState);
}
