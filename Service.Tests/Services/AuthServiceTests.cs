using Common.Enums;
using Common.Messages;
using Microsoft.Extensions.Configuration;
using Moq;
using OtpNet;
using Service.Interfaces;
using Service.Models.Auth;
using Service.Models.Role;
using Service.Models.User;
using Service.Services;

namespace Service.Tests.Services;

public class AuthServiceTests
{
    private readonly UserDto _userDto;

    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IRecoveryCodeRepository> _mockRecoveryCodeRepository;
    private readonly Mock<IRoleRepository> _mockRoleRepository;
    private readonly Mock<IConfiguration> _mockConfiguration;

    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _userDto = DataFactory.CreateUserDto();

        _mockUserRepository = new Mock<IUserRepository>();
        _mockRecoveryCodeRepository = new Mock<IRecoveryCodeRepository>();
        _mockRoleRepository = new Mock<IRoleRepository>();
        _mockConfiguration = new Mock<IConfiguration>();

        _authService = new AuthService(_mockUserRepository.Object, _mockRecoveryCodeRepository.Object, _mockRoleRepository.Object, _mockConfiguration.Object);
    }

     [Fact]
    public async Task GenerateTokenJWTAsync_ShouldThrowKeyNotFoundException_WhenUserNotFound()
    {
        // Arrange
        _mockUserRepository
            .Setup(repo => repo.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((UserDto)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _authService.GenerateTokenJWTAsync("user@example.com"));
    }

    [Fact]
    public async Task GenerateTokenJWTAsync_ShouldReturnValidToken_WhenUserExists()
    {
        // Arrange
        _mockUserRepository
            .Setup(repo => repo.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(_userDto);

        _mockConfiguration
            .Setup(config => config["Jwt:Key"])
            .Returns("this_is_a_secret_key_with_128bits");
        _mockConfiguration
            .Setup(config => config["Jwt:Issuer"])
            .Returns("issuer");
        _mockConfiguration
            .Setup(config => config["Jwt:Audience"])
            .Returns("audience");

        // Act
        var response = await _authService.GenerateTokenJWTAsync("user@example.com");

        // Assert
        Assert.NotNull(response.Token);
        Assert.Equal("JwtSecurityToken", response.TokenType);
        Assert.True(response.ExpiryDate > DateTime.Now);
    }

    [Fact]
    public async Task MfaEnableAsync_ShouldThrowKeyNotFoundException_WhenUserNotFound()
    {
        // Arrange        
        _mockUserRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((UserDto)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _authService.MfaEnableAsync(new()
        {
            UserId = _userDto.Id,
            MfaCode = "123456"
        }));

        Assert.Equal(exception.Message, Messages.NotFound);
    }

    [Fact]
    public async Task MfaEnableAsync_ShouldThrowInvalidOperationException_WhenUserMfaEnabled()
    {
        // Arrange        
        _mockUserRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_userDto);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _authService.MfaEnableAsync(new()
        {
            UserId = _userDto.Id,
            MfaCode = "123456"
        }));

        Assert.Equal(exception.Message, Messages.MfaActivated);
    }

    [Fact]
    public async Task MfaEnableAsync_ShouldThrowInvalidOperationException_WhenInvalidMfaCode()
    {
        // Arrange    
        _userDto.MfaEnabled = false;            
        _mockUserRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_userDto);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _authService.MfaEnableAsync(new()
        {
            UserId = _userDto.Id,
            MfaCode = "123456"
        }));

        Assert.Equal(exception.Message, Messages.InvalidMfaCode);
    }

     [Fact]
    public async Task MfaEnableAsync_ShouldEnableMfa_WhenValidMfaCode()
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
        await _authService.MfaEnableAsync(new()
        {
            UserId = _userDto.Id,
            MfaCode = validCode
        });

        Assert.True(_userDto.MfaEnabled);
    }

    [Fact]
    public async Task MfaRegenerateRecoveryCodesAsync_ShouldThrowKeyNotFoundException_WhenUserNotFound()
    {
        // Arrange        
        _mockUserRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((UserDto)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _authService.MfaRegenerateRecoveryCodesAsync(new()
        {
            UserId = _userDto.Id,
            MfaCode = "123456"
        }));

        Assert.Equal(exception.Message, Messages.NotFound);
    }

    [Fact]
    public async Task MfaRegenerateRecoveryCodesAsync_ShouldThrowInvalidOperationException_WhenUserMfaSecretIsNull()
    {
        // Arrange        
        _userDto.MfaSecret = null;

        _mockUserRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_userDto);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _authService.MfaRegenerateRecoveryCodesAsync(new()
        {
            UserId = _userDto.Id,
            MfaCode = "123456"
        }));

        Assert.Equal(exception.Message, Messages.MfaDisabled);
    }

    [Fact]
    public async Task MfaRegenerateRecoveryCodesAsync_ShouldThrowInvalidOperationException_WhenUserMfaDisabled()
    {
        // Arrange        
        _userDto.MfaEnabled = false;

        _mockUserRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_userDto);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _authService.MfaRegenerateRecoveryCodesAsync(new()
        {
            UserId = _userDto.Id,
            MfaCode = "123456"
        }));

        Assert.Equal(exception.Message, Messages.MfaDisabled);
    }

    [Fact]
    public async Task MfaRegenerateRecoveryCodesAsync_ShouldThrowInvalidOperationException_WhenInvalidMfaCode()
    {
        // Arrange    
        _mockUserRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_userDto);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _authService.MfaRegenerateRecoveryCodesAsync(new()
        {
            UserId = _userDto.Id,
            MfaCode = "123456"
        }));

        Assert.Equal(exception.Message, Messages.InvalidMfaCode);
    }

    [Fact]
    public async Task MfaRegenerateRecoveryCodesAsync_ShouldGenerateRecoveryCodes_WhenValidMfaCode()
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
        var result = await  _authService.MfaRegenerateRecoveryCodesAsync(new()
        {
            UserId = _userDto.Id,
            MfaCode = validCode
        });

        Assert.NotNull(result);
        Assert.Equal(8, result.Count);
    }

    [Fact]
    public async Task MfaSetupAsync_ShouldThrowKeyNotFoundException_WhenUserNotFound()
    {
        // Arrange        
        _mockUserRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((UserDto)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _authService.MfaSetupAsync(Guid.Parse("00000000-0000-0000-0000-000000000001")));

        Assert.Equal(exception.Message, Messages.NotFound);
    }

    [Fact]
    public async Task MfaSetupAsync_ShouldThrowInvalidOperationException_WhenMfaEnabled()
    {
        // Arrange
        _mockUserRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_userDto);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _authService.MfaSetupAsync(_userDto.Id));

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
        var response = await _authService.MfaSetupAsync(_userDto.Id);

        // Assert
        Assert.Equal(response.MfaQrCodeUri, $"otpauth://totp/Valeting:{Uri.EscapeDataString(_userDto.Email)}?secret={_userDto.MfaSecret}&issuer=Valeting");
        Assert.Null(response.RecoveryCodes);
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
        var response = await _authService.MfaSetupAsync(_userDto.Id);

        // Assert
        Assert.Equal(response.MfaQrCodeUri, $"otpauth://totp/Valeting:{Uri.EscapeDataString(_userDto.Email)}?secret={_userDto.MfaSecret}&issuer=Valeting");
        Assert.NotEmpty(response.RecoveryCodes);
        Assert.Equal(8, response.RecoveryCodes.Count);

        _mockUserRepository.Verify();
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrowInvalidOperationException_WhenUserExists()
    {
        // Arrange
        _mockUserRepository
            .Setup(repo => repo.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(_userDto);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _authService.RegisterAsync(_userDto));

        Assert.Equal(exception.Message, Messages.EmailInUse);
    }

    [Fact]
    public async Task RegisterAsync_ShouldCreateUser_WhenValid()
    {
        // Arrange
        _mockUserRepository
            .Setup(repo => repo.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((UserDto)null);

        _mockRoleRepository
            .Setup(repo => repo.GetByCodeAsync(It.IsAny<RoleEnum>()))
            .ReturnsAsync(new RoleDto
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                Code = RoleEnum.USER
            });

        // Act
        await _authService.RegisterAsync(new()
        {
            Email = "user@example.com",
            Password = "password",
            FirstName = "John1",
            LastName = "Doe",
            DateOfBirth = new DateOnly(1930, 3, 7),
            ContactNumber = 123456789,
            Role = new()
            {
                Code = RoleEnum.USER
            }
        });

        // Assert
        _mockUserRepository.Verify(repo => repo.RegisterAsync(It.IsAny<UserDto>()), Times.Once);
    }

    [Fact]
    public async Task ValidateLoginAsync_ShouldThrowKeyNotFoundException_WhenUserNotFound()
    {
        // Arrange
        _mockUserRepository
            .Setup(repo => repo.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((UserDto)null)
            .Verifiable(Times.Once);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _authService.ValidateLoginAsync(
                new()
                {
                    Email = "user@example.com",
                    Password = "password123"
                }));

        _mockUserRepository.Verify();
    }

    [Fact]
    public async Task ValidateLoginAsync_ShouldReturnInValid_WhenPasswordDoesNotMatches()
    {
        // Arrange
        _mockUserRepository
            .Setup(repo => repo.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(_userDto)
            .Verifiable(Times.Once);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _authService.ValidateLoginAsync(
            new()
            {
                Email = "user@example.com",
                Password = "password"
            }));

        _mockUserRepository.Verify();
    }

    [Fact]
    public async Task ValidateLoginAsync_ShouldReturnValid_WhenPasswordMatches()
    {
        // Arrange
        _mockUserRepository
            .Setup(repo => repo.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(_userDto)
            .Verifiable(Times.Once);

        // Act
        await _authService.ValidateLoginAsync(
            new()
            {
                Email = "user@example.com",
                Password = "password123"
            });

        // Assert
        _mockUserRepository.Verify();
    }

    [Fact]
    public async Task ValidateToken_ShouldReturnEmail_WhenTokenIsValid()
    {
        // Arrange
        _mockConfiguration
            .Setup(config => config["Jwt:Key"])
            .Returns("this_is_a_secret_key_with_128bits");
        _mockConfiguration
            .Setup(config => config["Jwt:Issuer"])
            .Returns("issuer");
        _mockConfiguration
            .Setup(config => config["Jwt:Audience"])
            .Returns("audience");

        _mockUserRepository
            .Setup(repo => repo.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(_userDto);

        var tokenResponse = await _authService.GenerateTokenJWTAsync("user@example.com");
        var validToken = tokenResponse.Token;

        // Act
        var email = _authService.ValidateToken(validToken);

        // Assert
        Assert.Equal("user@example.com", email);
    }
}
