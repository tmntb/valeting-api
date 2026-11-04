using Common.Messages;
using Moq;
using OtpNet;
using Service.Interfaces;
using Service.Models.Auth;
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
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
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
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_userDto);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _recoveryCodeService.GenerateRecoveryCodesAsync(_userDto.Id));

        Assert.Equal(exception.Message, Messages.MfaDisabled);
    }

    [Fact]
    public async Task RecoveryCodesGenerateAsync_ShouldThrowInvalidOperationException_WhenUserMfaDisabled()
    {
        // Arrange        
        _userDto.MfaEnabled = false;

        _mockUserRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_userDto);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _recoveryCodeService.GenerateRecoveryCodesAsync(_userDto.Id));

        Assert.Equal(exception.Message, Messages.MfaDisabled);
    }

    [Fact]
    public async Task RecoveryCodesGenerateAsync_ShouldGenerateRecoveryCodes_WhenValidMfaCode()
    {
        // Arrange    
        var secretBytes = Base32Encoding.ToBytes(_userDto.MfaSecret);
        var totp = new Totp(secretBytes);
        var validCode = totp.ComputeTotp();

        _mockUserRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
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
}
