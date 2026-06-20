using Repository.Repositories;
using Service.Models.Auth;

namespace Integration.Tests.Repository;

public class RecoveryCodeRepositoryTests  : BaseRepositoryTest
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

        var result = await Context.RecoveryCodes.FindAsync(_recoveryCodeDto.User.Id);

        // Assert
        Assert.Null(result);
    }
}
