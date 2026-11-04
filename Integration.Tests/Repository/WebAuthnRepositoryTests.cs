using Repository.Repositories;
using Service.Models.WebAuthn;

namespace Integration.Tests.Repository;

public class WebAuthnRepositoryTests : BaseRepositoryTest
{
    private readonly WebAuthnRepository _repository;

    public WebAuthnRepositoryTests()
    {
        _repository = new WebAuthnRepository(Context);
    }

    [Fact]
    public async Task CreateAsync_PersistsCredentialAndMapsAllFields()
    {
        // Arrange
        var webAuthnDto = DataFactory.CreateWebAuthnDto();

        // Act
        await _repository.CreateAsync(webAuthnDto);

        var entity = await Context.WebAuthns.FindAsync(webAuthnDto.Id);

        // Assert
        Assert.NotNull(entity);
        Assert.Equal(webAuthnDto.UserId, entity.UserId);
        Assert.Equal(webAuthnDto.CredentialId, entity.CredentialId);
        Assert.Equal(webAuthnDto.PublicKey, entity.PublicKey);
        Assert.Equal(webAuthnDto.FriendlyName, entity.FriendlyName);
        Assert.Equal(webAuthnDto.Aaguid, entity.Aaguid);
        Assert.Equal(webAuthnDto.Transports, entity.Transports);
        Assert.Equal(webAuthnDto.CreatedAt, entity.CreatedAt);
        Assert.Equal(webAuthnDto.LastUsedAt, entity.LastUsedAt);
        Assert.Equal(webAuthnDto.SignatureCounter, entity.SignatureCounter);
    }

    [Fact]
    public async Task GetByUserIdAsync_ReturnsOnlyUserCredentialsInCreationOrder()
    {
        // Arrange
        var newer = DataFactory.CreateWebAuthnDto(Guid.NewGuid(), "newer", DateTime.UtcNow);
        var older = DataFactory.CreateWebAuthnDto(Guid.NewGuid(), "older", newer.CreatedAt.AddMinutes(-1));
        var anotherUser = DataFactory.CreateWebAuthnDto(Guid.NewGuid(), "other-user", newer.CreatedAt, Guid.NewGuid());
        await _repository.CreateAsync(newer);
        await _repository.CreateAsync(anotherUser);
        await _repository.CreateAsync(older);

        // Act
        var result = (await _repository.GetByUserIdAsync(DataFactory.USER_ID)).ToList();

        // Assert
        Assert.Collection(result,
            first => Assert.Equal(older.CredentialId, first.CredentialId),
            second => Assert.Equal(newer.CredentialId, second.CredentialId));
    }

    [Fact]
    public async Task GetByCredentialIdAsync_ReturnsMappedCredentialOrNull()
    {
        // Arrange
        var credential = DataFactory.CreateWebAuthnDto();
        await _repository.CreateAsync(credential);

        // Act
        var result = await _repository.GetByCredentialIdAsync(credential.CredentialId);
        var missing = await _repository.GetByCredentialIdAsync("missing");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(credential.Id, result.Id);
        Assert.Equal(credential.UserId, result.UserId);
        Assert.Equal(credential.CredentialId, result.CredentialId);
        Assert.Equal(credential.PublicKey, result.PublicKey);
        Assert.Equal(credential.FriendlyName, result.FriendlyName);
        Assert.Equal(credential.Aaguid, result.Aaguid);
        Assert.Equal(credential.Transports, result.Transports);
        Assert.Equal(credential.CreatedAt, result.CreatedAt);
        Assert.Equal(credential.LastUsedAt, result.LastUsedAt);
        Assert.Equal(credential.SignatureCounter, result.SignatureCounter);
        Assert.Null(missing);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesMutableCredentialFields()
    {
        // Arrange
        var original = DataFactory.CreateWebAuthnDto();
        await _repository.CreateAsync(original);
        var updated = new WebAuthnDto
        {
            Id = original.Id,
            UserId = Guid.NewGuid(),
            CredentialId = "updated-credential",
            PublicKey = "updated-public-key",
            FriendlyName = "Updated name",
            Aaguid = "updated-aaguid",
            Transports = """["internal"]""",
            CreatedAt = original.CreatedAt.AddDays(1),
            LastUsedAt = DateTime.UtcNow,
            SignatureCounter = 7
        };

        // Act
        await _repository.UpdateAsync(updated);

        var entity = await Context.WebAuthns.FindAsync(original.Id);
        // Assert
        Assert.NotNull(entity);
        Assert.Equal(original.UserId, entity.UserId);
        Assert.Equal(original.CreatedAt, entity.CreatedAt);
        Assert.Equal(updated.CredentialId, entity.CredentialId);
        Assert.Equal(updated.PublicKey, entity.PublicKey);
        Assert.Equal(updated.FriendlyName, entity.FriendlyName);
        Assert.Equal(updated.Aaguid, entity.Aaguid);
        Assert.Equal(updated.Transports, entity.Transports);
        Assert.Equal(updated.LastUsedAt, entity.LastUsedAt);
        Assert.Equal(updated.SignatureCounter, entity.SignatureCounter);
    }

    [Fact]
    public async Task UpdateAsync_DoesNothingWhenCredentialDoesNotExist()
    {
        // Act
        await _repository.UpdateAsync(DataFactory.CreateWebAuthnDto(Guid.NewGuid()));

        // Assert
        Assert.Empty(Context.WebAuthns);
    }

    [Fact]
    public async Task UpdateSignalCounter_UpdatesCounterAndLastUsedAt()
    {
        // Arrange
        var credential = DataFactory.CreateWebAuthnDto();
        await _repository.CreateAsync(credential);
        credential.SignatureCounter = 12;

        var beforeUpdate = DateTime.UtcNow;
        // Act
        await _repository.UpdateSignalCounter(credential);
        var afterUpdate = DateTime.UtcNow;

        var entity = await Context.WebAuthns.FindAsync(credential.Id);
        // Assert
        Assert.NotNull(entity);
        Assert.Equal(12, entity.SignatureCounter);
        Assert.NotNull(entity.LastUsedAt);
        Assert.InRange(entity.LastUsedAt.Value, beforeUpdate, afterUpdate);
    }

    [Fact]
    public async Task DeleteAsync_RemovesCredentialAndIgnoresMissingId()
    {
        // Arrange
        var credential = DataFactory.CreateWebAuthnDto();
        await _repository.CreateAsync(credential);

        // Act
        await _repository.DeleteAsync(credential.Id);

        // Assert
        Assert.Null(await Context.WebAuthns.FindAsync(credential.Id));
        await _repository.DeleteAsync(Guid.NewGuid());
        Assert.Empty(Context.WebAuthns);
    }

    
}
