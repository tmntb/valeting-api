using Common.Messages;
using Moq;
using OtpNet;
using Service.Interfaces;
using Service.Models.User;
using Service.Services;

namespace Service.Tests.Services;

public class MfaServiceTests
{
    private readonly UserDto _userDto;


    private readonly Mock<IUserRepository> _mockUserRepository;

    private readonly MfaService _mfaService;

    public MfaServiceTests()
    {
        _userDto = DataFactory.CreateUserDto();

        _mockUserRepository = new Mock<IUserRepository>();

        _mfaService = new MfaService(_mockUserRepository.Object);
    }

    [Fact]
    public async Task MfaVerifyAsync_ShouldThrowKeyNotFoundException_WhenUserNotFound()
    {
        // Arrange        
        _mockUserRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((UserDto)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _mfaService.MfaVerifyAsync(new()
        {
            UserId = _userDto.Id,
            MfaCode = "123456"
        }));

        Assert.Equal(exception.Message, Messages.NotFound);
    }

    [Fact]
    public async Task MfaVerifyAsync_ShouldThrowInvalidOperationException_WhenInvalidMfaCode()
    {
        // Arrange    
        _userDto.MfaEnabled = false;
        _mockUserRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_userDto);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _mfaService.MfaVerifyAsync(new()
        {
            UserId = _userDto.Id,
            MfaCode = "123456"
        }));

        Assert.Equal(exception.Message, Messages.InvalidMfaCode);
    }

    [Fact]
    public async Task MfaVerifyAsync_ShouldEnableMfa_WhenValidMfaCode()
    {
        // Arrange 
        _userDto.MfaEnabled = false;

        var secretBytes = Base32Encoding.ToBytes(_userDto.MfaSecret);
        var totp = new Totp(secretBytes);
        var validCode = totp.ComputeTotp();

        _mockUserRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_userDto);

        // Act & Assert
        await _mfaService.MfaVerifyAsync(new()
        {
            UserId = _userDto.Id,
            MfaCode = validCode
        });

        Assert.True(_userDto.MfaEnabled);
    }

    [Fact]
    public async Task MfaSetupAsync_ShouldThrowKeyNotFoundException_WhenUserNotFound()
    {
        // Arrange        
        _mockUserRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((UserDto)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _mfaService.MfaSetupAsync(Guid.Parse("00000000-0000-0000-0000-000000000001")));

        Assert.Equal(exception.Message, Messages.NotFound);
    }

    [Fact]
    public async Task MfaSetupAsync_ShouldThrowInvalidOperationException_WhenMfaVerify()
    {
        // Arrange
        _mockUserRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_userDto);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _mfaService.MfaSetupAsync(_userDto.Id));

        Assert.Equal(exception.Message, Messages.MfaActivated);
    }

    [Fact]
    public async Task MfaSetupAsync_ShouldReturnExistingMfaCodeUri_WhenMfaNotEnabledAndMfaSecretExists()
    {
        // Arrange
        _userDto.MfaEnabled = false;

        _mockUserRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_userDto);

        // Act
        var response = await _mfaService.MfaSetupAsync(_userDto.Id);

        // Assert
        Assert.Equal(response.MfaQrCodeUri, $"otpauth://totp/Valeting:{Uri.EscapeDataString(_userDto.Email)}?secret={_userDto.MfaSecret}&issuer=Valeting");
    }

    [Fact]
    public async Task MfaSetupAsync_ShouldGenerateMfaSecretAndRecoveryCodes_WhenMfaNotEnabledAndMfaSecretNotExists()
    {
        // Arrange
        _userDto.MfaEnabled = false;
        _userDto.MfaSecret = null;

        _mockUserRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_userDto)
            .Verifiable(Times.Once);

        _mockUserRepository
            .Setup(repo => repo.UpdateMfaSecretAsync(It.IsAny<UserDto>()))
            .Returns(Task.CompletedTask)
            .Verifiable(Times.Once);

        // Act
        var response = await _mfaService.MfaSetupAsync(_userDto.Id);

        // Assert
        Assert.Equal(response.MfaQrCodeUri, $"otpauth://totp/Valeting:{Uri.EscapeDataString(_userDto.Email)}?secret={_userDto.MfaSecret}&issuer=Valeting");

        _mockUserRepository.Verify();
    }
}
