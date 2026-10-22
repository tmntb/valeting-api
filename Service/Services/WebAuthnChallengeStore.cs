using System.Collections.Concurrent;
using Service.Interfaces;
using Service.Models.WebAuthn;

namespace Service.Services;

public sealed class WebAuthnChallengeStore : IWebAuthnChallengeStore
{
    private readonly ConcurrentDictionary<Guid, PasskeyChallengeState> _challenges = new();

    /// <inheritdoc />
    public void Store(PasskeyChallengeState state)
    {
        foreach (var challenge in _challenges)
        {
            if (challenge.Value.ExpiresAt <= DateTimeOffset.UtcNow)
            {
                _challenges.TryRemove(challenge.Key, out _);
            }
        }

        _challenges[state.CeremonyId] = state;
    }

    /// <inheritdoc />
    public bool TryConsume(Guid ceremonyId, PasskeyCeremony ceremony, out PasskeyChallengeState? state)
    {
        if (!_challenges.TryRemove(ceremonyId, out state))
        {
            return false;
        }

        if (state.Ceremony != ceremony || state.ExpiresAt <= DateTimeOffset.UtcNow)
        {
            state = null;
            return false;
        }

        return true;
    }
}
