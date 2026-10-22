using Service.Models.WebAuthn;
using Service.Services;

namespace Service.Tests.Services;

public class WebAuthnChallengeStoreTests
{
    [Fact]
    public void TryConsume_ReturnsStateOnlyOnce()
    {
        var store = new WebAuthnChallengeStore();
        var state = CreateState();
        store.Store(state);

        Assert.True(store.TryConsume(state.CeremonyId, state.Ceremony, out var consumed));
        Assert.Same(state, consumed);
        Assert.False(store.TryConsume(state.CeremonyId, state.Ceremony, out _));
    }

    [Fact]
    public void TryConsume_RejectsExpiredChallenge()
    {
        var store = new WebAuthnChallengeStore();
        var state = new PasskeyChallengeState
        {
            CeremonyId = Guid.NewGuid(),
            Ceremony = PasskeyCeremony.Authentication,
            OptionsJson = "{}",
            ExpiresAt = DateTimeOffset.UtcNow.AddSeconds(-1)
        };
        store.Store(state);

        Assert.False(store.TryConsume(state.CeremonyId, state.Ceremony, out _));
    }

    [Fact]
    public void TryConsume_AllowsOnlyOneConcurrentConsumer()
    {
        var store = new WebAuthnChallengeStore();
        var state = CreateState();
        store.Store(state);
        var successfulConsumers = 0;

        Parallel.For(0, 32, index =>
        {
            if (store.TryConsume(state.CeremonyId, state.Ceremony, out var consumed))
            {
                Interlocked.Increment(ref successfulConsumers);
            }
        });

        Assert.Equal(1, successfulConsumers);
    }

    private static PasskeyChallengeState CreateState() => new()
    {
        CeremonyId = Guid.NewGuid(),
        Ceremony = PasskeyCeremony.Authentication,
        OptionsJson = "{}",
        ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(1)
    };
}
