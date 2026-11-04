using System.Collections.Concurrent;
using Service.Interfaces;
using Service.Models.WebAuthn;

namespace Service.Services;

public sealed class WebAuthnChallengeStore : IWebAuthnChallengeStore
{
    private readonly ConcurrentDictionary<Guid, WebAuthnChallengeState> _webAuthnChallenges = new();

    /// <inheritdoc />
    public void Store(WebAuthnChallengeState webAuthnChallengeState)
    {
        foreach (var challenge in _webAuthnChallenges)
        {
            if (challenge.Value.ExpiresAt <= DateTimeOffset.UtcNow)
            {
                _webAuthnChallenges.TryRemove(challenge.Key, out _);
            }
        }

        _webAuthnChallenges[webAuthnChallengeState.CeremonyId] = webAuthnChallengeState;
    }

    /// <inheritdoc />
    public bool TryConsume(Guid ceremonyId, WebAuthnCeremony webAuthnCeremony, out WebAuthnChallengeState? webAuthnChallengeState)
    {
        if (!_webAuthnChallenges.TryRemove(ceremonyId, out webAuthnChallengeState))
        {
            return false;
        }

        if (webAuthnChallengeState.Ceremony == webAuthnCeremony && webAuthnChallengeState.ExpiresAt > DateTimeOffset.UtcNow)
        {
            return true;
        }

        webAuthnChallengeState = null;
        return false;
    }
}
