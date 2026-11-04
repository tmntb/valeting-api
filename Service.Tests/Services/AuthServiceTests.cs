using Common.Enums;
using Common.Messages;
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

    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _userDto = DataFactory.CreateUserDto();

        _mockUserRepository = new Mock<IUserRepository>();
        _mockRecoveryCodeRepository = new Mock<IRecoveryCodeRepository>();
        _mockRoleRepository = new Mock<IRoleRepository>();

        _authService = new AuthService(_mockUserRepository.Object, _mockRecoveryCodeRepository.Object, _mockRoleRepository.Object);
    }

    [Fact]
    public async Task ForgotPasswordAsync_ShouldReturn_WhenUserDoesNotExist()
    {
        // Arrange
        _mockUserRepository
            .Setup(repo => repo.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((UserDto)null)
            .Verifiable(Times.Once);

        // Act
        await _authService.ForgotPasswordAsync(new()
        {
            Email = "user@example.com.pt",
            MfaCode = "123456",
            NewPassword = "newpassword123"
        });

        // Assert
        _mockUserRepository.Verify();
        _mockRecoveryCodeRepository.Verify(r => r.GetUserRecoveryCodesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockRecoveryCodeRepository.Verify(r => r.UpdateUsedAtAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUserRepository.Verify(r => r.UpdatePasswordAsync(It.IsAny<UserDto>()), Times.Never);
    }

    [Fact]
    public async Task ForgotPasswordAsync_ShouldThrowInvalidOperationException_WhenMfaCodeIsInvalid()
    {
        // Arrange
        _mockUserRepository
            .Setup(repo => repo.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(_userDto)
            .Verifiable(Times.Once);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _authService.ForgotPasswordAsync(new()
        {
            Email = "user@example.com.pt",
            MfaCode = "123456",
            NewPassword = "newpassword123"
        }));

        Assert.Equal(exception.Message, Messages.InvalidMfaCode);

        _mockUserRepository.Verify();
        _mockRecoveryCodeRepository.Verify(r => r.GetUserRecoveryCodesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockRecoveryCodeRepository.Verify(r => r.UpdateUsedAtAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUserRepository.Verify(r => r.UpdatePasswordAsync(It.IsAny<UserDto>()), Times.Never);
    }

    [Fact]
    public async Task ForgotPasswordAsync_ShouldUpdatePassword_WhenMfaCodeIsValid()
    {
        // Arrange
        var secretBytes = Base32Encoding.ToBytes(_userDto.MfaSecret);
        var totp = new Totp(secretBytes);
        var validCode = totp.ComputeTotp();

        _mockUserRepository
           .Setup(repo => repo.GetByEmailAsync(It.IsAny<string>()))
           .ReturnsAsync(_userDto)
           .Verifiable(Times.Once);

        // Act
        await _authService.ForgotPasswordAsync(new()
        {
            Email = "user@example.com.pt",
            MfaCode = validCode,
            NewPassword = "newpassword123"
        });

        // Assert
        _mockUserRepository.Verify();
        _mockRecoveryCodeRepository.Verify(r => r.GetUserRecoveryCodesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockRecoveryCodeRepository.Verify(r => r.UpdateUsedAtAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUserRepository.Verify(r => r.UpdatePasswordAsync(It.IsAny<UserDto>()), Times.Once);
    }

    [Fact]
    public async Task ForgotPasswordAsync_ShouldThrowInvalidOperationException_WhenRecoveryCodesIsNull()
    {
        // Arrange
        _mockUserRepository
           .Setup(repo => repo.GetByEmailAsync(It.IsAny<string>()))
           .ReturnsAsync(_userDto)
           .Verifiable(Times.Once);

        _mockRecoveryCodeRepository
            .Setup(r => r.GetUserRecoveryCodesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((List<RecoveryCodeDto>)null)
            .Verifiable(Times.Once);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _authService.ForgotPasswordAsync(new()
        {
            Email = "user@example.com.pt",
            RecoveryCode = "1234-5678",
            NewPassword = "newpassword123"
        }));

        Assert.Equal(exception.Message, Messages.NoRecoveryCodesForUser);

        _mockUserRepository.Verify();
        _mockRecoveryCodeRepository.Verify();
        _mockRecoveryCodeRepository.Verify(r => r.UpdateUsedAtAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUserRepository.Verify(r => r.UpdatePasswordAsync(It.IsAny<UserDto>()), Times.Never);
    }

    [Fact]
    public async Task ForgotPasswordAsync_ShouldThrowInvalidOperationException_WhenRecoveryCodesIsEmpty()
    {
        // Arrange
        _mockUserRepository
           .Setup(repo => repo.GetByEmailAsync(It.IsAny<string>()))
           .ReturnsAsync(_userDto)
           .Verifiable(Times.Once);

        _mockRecoveryCodeRepository
            .Setup(r => r.GetUserRecoveryCodesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([])
            .Verifiable(Times.Once);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _authService.ForgotPasswordAsync(new()
        {
            Email = "user@example.com.pt",
            RecoveryCode = "1234-5678",
            NewPassword = "newpassword123"
        }));

        Assert.Equal(exception.Message, Messages.NoRecoveryCodesForUser);

        _mockUserRepository.Verify();
        _mockRecoveryCodeRepository.Verify();
        _mockRecoveryCodeRepository.Verify(r => r.UpdateUsedAtAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUserRepository.Verify(r => r.UpdatePasswordAsync(It.IsAny<UserDto>()), Times.Never);
    }

    [Fact]
    public async Task ForgotPasswordAsync_ShouldThrowInvalidOperationException_WhenRecoveryCodeIsUsed()
    {
        // Arrange
        _mockUserRepository
           .Setup(repo => repo.GetByEmailAsync(It.IsAny<string>()))
           .ReturnsAsync(_userDto)
           .Verifiable(Times.Once);

        _mockRecoveryCodeRepository
            .Setup(r => r.GetUserRecoveryCodesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([DataFactory.CreateRecoveryCodeDto(usedAt: DateTime.UtcNow)])
            .Verifiable(Times.Once);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _authService.ForgotPasswordAsync(new()
        {
            Email = "user@example.com.pt",
            RecoveryCode = "1234-5678",
            NewPassword = "newpassword123"
        }));

        Assert.Equal(exception.Message, Messages.InvalidRecoveryCode);

        _mockUserRepository.Verify();
        _mockRecoveryCodeRepository.Verify();
        _mockRecoveryCodeRepository.Verify(r => r.UpdateUsedAtAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUserRepository.Verify(r => r.UpdatePasswordAsync(It.IsAny<UserDto>()), Times.Never);
    }

    [Fact]
    public async Task ForgotPasswordAsync_ShouldThrowInvalidOperationException_WhenRecoveryCodeIsInvalid()
    {
        // Arrange
        _mockUserRepository
           .Setup(repo => repo.GetByEmailAsync(It.IsAny<string>()))
           .ReturnsAsync(_userDto)
           .Verifiable(Times.Once);

        _mockRecoveryCodeRepository
            .Setup(r => r.GetUserRecoveryCodesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([DataFactory.CreateRecoveryCodeDto()])
            .Verifiable(Times.Once);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _authService.ForgotPasswordAsync(new()
        {
            Email = "user@example.com.pt",
            RecoveryCode = "1234-8765",
            NewPassword = "newpassword123"
        }));

        Assert.Equal(exception.Message, Messages.InvalidRecoveryCode);

        _mockUserRepository.Verify();
        _mockRecoveryCodeRepository.Verify();
        _mockRecoveryCodeRepository.Verify(r => r.UpdateUsedAtAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUserRepository.Verify(r => r.UpdatePasswordAsync(It.IsAny<UserDto>()), Times.Never);
    }

    [Fact]
    public async Task ForgotPasswordAsync_ShouldUpdatePassword_WhenRecoveryCodeIsValid()
    {
        // Arrange
        _mockUserRepository
           .Setup(repo => repo.GetByEmailAsync(It.IsAny<string>()))
           .ReturnsAsync(_userDto)
           .Verifiable(Times.Once);

        _mockRecoveryCodeRepository
            .Setup(r => r.GetUserRecoveryCodesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([DataFactory.CreateRecoveryCodeDto()])
            .Verifiable(Times.Once);

        _mockRecoveryCodeRepository
            .Setup(r => r.UpdateUsedAtAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable(Times.Once);

        // Act
        await _authService.ForgotPasswordAsync(new()
        {
            Email = "user@example.com.pt",
            RecoveryCode = "1234-5678",
            NewPassword = "newpassword123"
        });

        // Assert
        _mockUserRepository.Verify();
        _mockRecoveryCodeRepository.Verify();
        _mockUserRepository.Verify(r => r.UpdatePasswordAsync(It.IsAny<UserDto>()), Times.Once);
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
}
