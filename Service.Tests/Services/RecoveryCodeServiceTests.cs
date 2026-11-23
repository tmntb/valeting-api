using Common.Messages;
using Moq;
using Service.Interfaces;
using Service.Models.RecoveryCode;
using Service.Models.RecoveryCode.Payload;
using Service.Models.User;
using Service.Services;

namespace Service.Tests.Services;

public class RecoveryCodeServiceTests
{
    private readonly UserDto _userDto;

    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IRecoveryCodeRepository> _mockRecoveryCodeRepository;
    
    private readonly RecoveryCodeService _recoveryCodeService;
    
    public RecoveryCodeServiceTests()
    {
        _userDto = DataFactory.CreateUserDto();

        _mockUserRepository = new Mock<IUserRepository>();
        _mockRecoveryCodeRepository = new Mock<IRecoveryCodeRepository>();

        _recoveryCodeService = new RecoveryCodeService(_mockUserRepository.Object, _mockRecoveryCodeRepository.Object);
    }

    [Fact]
    public async Task RecoveryCodesGenerateAsync_ShouldThrowKeyNotFoundException_WhenUserNotFound()
    {
        // Arrange        
        _mockUserRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserDto)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _recoveryCodeService.GenerateRecoveryCodesAsync(_userDto.Id));

        Assert.Equal(exception.Message, Messages.NotFound);
    }

    [Fact]
    public async Task RecoveryCodesGenerateAsync_ShouldThrowInvalidOperationException_WhenUserMfaSecretIsNull()
    {
        // Arrange        
        _userDto.MfaSecret = null;

        _mockUserRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_userDto);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _recoveryCodeService.GenerateRecoveryCodesAsync(_userDto.Id));

        Assert.Equal(exception.Message, Messages.MfaDisabled);
    }

    [Fact]
    public async Task RecoveryCodesGenerateAsync_ShouldThrowInvalidOperationException_WhenUserMfaDisabled()
    {
        // Arrange        
        _userDto.TwoFactorAuthEnabled = false;

        _mockUserRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_userDto);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _recoveryCodeService.GenerateRecoveryCodesAsync(_userDto.Id));

        Assert.Equal(exception.Message, Messages.MfaDisabled);
    }

    [Fact]
    public async Task RecoveryCodesGenerateAsync_ShouldGenerateRecoveryCodes_WhenValidMfaCode()
    {
        // Arrange    
        _mockUserRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_userDto);

        _mockRecoveryCodeRepository
            .Setup(x => x.DeleteManyAsync(It.IsAny<Guid>()))
            .Returns(Task.CompletedTask);

        _mockRecoveryCodeRepository
            .Setup(x => x.CreateManyAsync(It.IsAny<List<RecoveryCodeDto>>()))
            .Returns(Task.CompletedTask);

        // Act & Assert
        var result = await _recoveryCodeService.GenerateRecoveryCodesAsync(_userDto.Id);

        Assert.NotNull(result);
        Assert.Equal(8, result.Count);
    }

    [Fact]
    public async Task VerifyRecoveryCodeAsync_ShouldThrowWhenUserHasNoRecoveryCodes()
    {
        _mockUserRepository
            .Setup(repo => repo.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_userDto);
        _mockRecoveryCodeRepository
            .Setup(repo => repo.GetUserRecoveryCodesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _recoveryCodeService.VerifyRecoveryCodeAsync(new VerifyRecoveryCodeDtoRequest
            {
                Email = _userDto.Email,
                Code = "1234-5678"
            }));

        Assert.Equal(Messages.NoRecoveryCodesForUser, exception.Message);
        _mockRecoveryCodeRepository.Verify(repo =>
            repo.UpdateUsedAtAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task VerifyRecoveryCodeAsync_ShouldThrowWhenCodeIsInvalid()
    {
        var recoveryCode = DataFactory.CreateRecoveryCodeDto();
        _mockUserRepository
            .Setup(repo => repo.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_userDto);
        _mockRecoveryCodeRepository
            .Setup(repo => repo.GetUserRecoveryCodesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([recoveryCode]);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _recoveryCodeService.VerifyRecoveryCodeAsync(new VerifyRecoveryCodeDtoRequest
            {
                Email = _userDto.Email,
                Code = "8765-4321"
            }));

        Assert.Equal(Messages.InvalidRecoveryCode, exception.Message);
        _mockRecoveryCodeRepository.Verify(repo =>
            repo.UpdateUsedAtAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task VerifyRecoveryCodeAsync_ShouldMarkMatchingUnusedCodeAsUsed()
    {
        var recoveryCode = DataFactory.CreateRecoveryCodeDto();
        _mockUserRepository
            .Setup(repo => repo.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_userDto);
        _mockRecoveryCodeRepository
            .Setup(repo => repo.GetUserRecoveryCodesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([recoveryCode]);
        _mockRecoveryCodeRepository
            .Setup(repo => repo.UpdateUsedAtAsync(recoveryCode.Id, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _recoveryCodeService.VerifyRecoveryCodeAsync(new VerifyRecoveryCodeDtoRequest
        {
            Email = _userDto.Email,
            Code = "1234-5678"
        });

        _mockRecoveryCodeRepository.Verify(repo =>
            repo.UpdateUsedAtAsync(recoveryCode.Id, It.IsAny<CancellationToken>()), Times.Once);
    }
}
