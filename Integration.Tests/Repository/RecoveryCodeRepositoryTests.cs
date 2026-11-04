using Repository.Repositories;
using Service.Models.Auth;

namespace Integration.Tests.Repository;

public class RecoveryCodeRepositoryTests : BaseRepositoryTest
{
    private readonly RecoveryCodeDto _recoveryCodeDto;
    private readonly RecoveryCodeRepository _recoveryCodeRepository;

    public RecoveryCodeRepositoryTests()
    {
        _recoveryCodeDto = DataFactory.CreateRecoveryCodeDto();

        _recoveryCodeRepository = new RecoveryCodeRepository(Context);
    }

    [Fact]
    public async Task CreateManyAsync_ShouldAddRecoveryCodeToDatabase()
    {
        // Act
        await _recoveryCodeRepository.CreateManyAsync([_recoveryCodeDto]);

        var result = await Context.RecoveryCodes.FindAsync(_recoveryCodeDto.Id);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task DeleteManyAsync_ShouldRemoveRecoveryCodeFromDatabase()
    {
        // Arrange
        await _recoveryCodeRepository.CreateManyAsync([_recoveryCodeDto]);

        // Act
        await _recoveryCodeRepository.DeleteManyAsync(_recoveryCodeDto.User.Id);

        // Assert
        var result = await Context.RecoveryCodes.FindAsync(_recoveryCodeDto.User.Id);
        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserRecoveryCodesAsync_ShouldReturnNull_WhenNoRecoveryCodesExistForTheUser()
    {
        // Arrange
        await _recoveryCodeRepository.CreateManyAsync([_recoveryCodeDto]);

        // Act
        var result = await _recoveryCodeRepository.GetUserRecoveryCodesAsync(Guid.Parse("00000000-0000-0000-0000-000000000015"));

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserRecoveryCodesAsync_ShouldReturnRecoveryCodes_WhenRecoveryCodesExistForTheUser()
    {
        // Arrange
        await _recoveryCodeRepository.CreateManyAsync([_recoveryCodeDto]);

        // Act
        var result = await _recoveryCodeRepository.GetUserRecoveryCodesAsync(_recoveryCodeDto.User.Id);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task UpdateUsedAtAsync_ShouldReturn_WhenNoRecoveryCodesExistForGivenId()
    {
        // Arrange
        await _recoveryCodeRepository.CreateManyAsync([DataFactory.CreateRecoveryCodeDto(userId: Guid.NewGuid())]);

        // Act
        await _recoveryCodeRepository.UpdateUsedAtAsync(Guid.Parse("00000000-0000-0000-0000-000000000017"));

        // Assert
        var result = await Context.RecoveryCodes.FindAsync(Guid.Parse("00000000-0000-0000-0000-000000000017"));
        
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateUsedAtAsync_ShouldUpdateUsedAt_WhenRecoveryCodesExistForGivenId()
    {
        // Arrange
        await _recoveryCodeRepository.CreateManyAsync([_recoveryCodeDto]);

        // Act
        await _recoveryCodeRepository.UpdateUsedAtAsync(_recoveryCodeDto.Id);

        // Assert
        var result = await Context.RecoveryCodes.FindAsync(_recoveryCodeDto.Id);

        Assert.NotNull(result);
        Assert.NotNull(result.UsedAt);
    }
}
