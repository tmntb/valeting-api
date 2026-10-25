using Service.Models.WebAuthn;
using Service.Services;

namespace Service.Tests.Services;

public class WebAuthnChallengeStoreTests
{
    [Fact]
    public void TryConsume_ReturnsStateOnlyOnce()
    {
        // Arrange
        var store = new WebAuthnChallengeStore();
        var state = CreateState();

        // Act
        store.Store(state);

        // Assert
        Assert.True(store.TryConsume(state.CeremonyId, state.Ceremony, out var consumed));
        Assert.Same(state, consumed);
        Assert.False(store.TryConsume(state.CeremonyId, state.Ceremony, out _));
    }

    [Fact]
    public void TryConsume_RejectsExpiredChallenge()
    {
        // Arrange
        var store = new WebAuthnChallengeStore();
        var state = new WebAuthnChallengeState

        // Act
        {
            CeremonyId = Guid.NewGuid(),
            Ceremony = WebAuthnCeremony.Authentication,
            OptionsJson = "{}",
            ExpiresAt = DateTimeOffset.UtcNow.AddSeconds(-1)
        };
        store.Store(state);

        // Assert
        Assert.False(store.TryConsume(state.CeremonyId, state.Ceremony, out _));
    }

    [Fact]
    public void TryConsume_RejectsWrongCeremonyAndConsumesChallenge()
    {
        // Arrange
        var store = new WebAuthnChallengeStore();
        var state = CreateState();

        // Act
        store.Store(state);

        // Assert
        Assert.False(store.TryConsume(state.CeremonyId, WebAuthnCeremony.Registration, out var rejected));
        Assert.Null(rejected);
        Assert.False(store.TryConsume(state.CeremonyId, state.Ceremony, out _));
    }

    [Fact]
    public void TryConsume_AllowsOnlyOneConcurrentConsumer()
    {
        // Arrange
        var store = new WebAuthnChallengeStore();
        var state = CreateState();

        // Act
        store.Store(state);
        var successfulConsumers = 0;

        Parallel.For(0, 32, index =>
        {
            if (store.TryConsume(state.CeremonyId, state.Ceremony, out var consumed))
            {
                Interlocked.Increment(ref successfulConsumers);
            }
        });

        // Assert
        Assert.Equal(1, successfulConsumers);
    }

    [Fact]
    public void Store_ReplacesChallengeWithSameCeremonyId()
    {
        // Arrange
        var store = new WebAuthnChallengeStore();
        var ceremonyId = Guid.NewGuid();

        // Act
        var original = CreateState(ceremonyId, DateTimeOffset.UtcNow.AddMinutes(-1));
        var replacement = new WebAuthnChallengeState
        {
            CeremonyId = ceremonyId,
            Ceremony = WebAuthnCeremony.Authentication,
            OptionsJson = """{"replacement":true}""",
            ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(1)
        };

        store.Store(original);
        store.Store(replacement);

        // Assert
        Assert.True(store.TryConsume(ceremonyId, replacement.Ceremony, out var consumed));
        Assert.Same(replacement, consumed);
        Assert.Equal("""{"replacement":true}""", consumed.OptionsJson);
    }

    private static WebAuthnChallengeState CreateState(Guid ceremonyId = default, DateTimeOffset? expiresAt = null) => new()
    {
        CeremonyId = ceremonyId == default ? Guid.NewGuid() : ceremonyId,
        Ceremony = WebAuthnCeremony.Authentication,
        OptionsJson = "{}",
        ExpiresAt = expiresAt ?? DateTimeOffset.UtcNow.AddMinutes(1)
    };
}
