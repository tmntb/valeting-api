using Common.Messages;
using Microsoft.Extensions.Configuration;
using Moq;
using Service.Interfaces;
using Service.Models.User;
using Service.Services;

namespace Service.Tests.Services;

public class UserServiceTests
{
    private readonly UserDto _userDto;

    private readonly Mock<IUserRepository> _mockUserRepository;

    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userDto = DataFactory.CreateUserDto();

        _mockUserRepository = new Mock<IUserRepository>();
        _userService = new UserService(_mockUserRepository.Object);
    }

    [Fact]
    public async Task ResetAsync_ShouldThrowKeyNotFoundException_WhenUserNotFound()
    {
        // Arrange
        _mockUserRepository
            .Setup(repo => repo.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((UserDto)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _userService.ResetAsync(
                new()
                {
                    Code = "resetcode",
                    Email = "user@example.com",
                    NewPassword = "newpassword"
                }));
    }

    [Fact]
    public async Task ResetAsync_ShouldUpdatePassword_WhenUserExists()
    {
        // Arrange
        _mockUserRepository
            .Setup(repo => repo.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(_userDto);

        // Act
        await _userService.ResetAsync(
            new()
            {
                Code = "resetcode",
                Email = "user@example.com",
                NewPassword = "newpassword"
            });

        // Assert
        _mockUserRepository.Verify(x => x.UpdatePasswordAsync(It.IsAny<UserDto>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAdminSettingsAsync_ShouldThrowKeyNotFoundException_WhenUserNotFound()
    {
        // Arrange
        _mockUserRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((UserDto)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _userService.UpdateAdminSettingsAsync(
                new()
                {
                    UserId = _userDto.Id,
                    IsActive = true,
                    RoleId = _userDto.Role.Id
                }));
    }

    [Fact]
    public async Task UpdateAdminSettingsAsync_ShouldUpdateAdminSettings_WhenUserExists()
    {
        // Arrange
        _mockUserRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_userDto)
            .Verifiable(Times.Once);

        _mockUserRepository
            .Setup(repo => repo.UpdateAdminSettingsAsync(It.IsAny<UserDto>()))
            .Returns(Task.CompletedTask)
            .Verifiable(Times.Once);

        // Act
        await _userService.UpdateAdminSettingsAsync(
            new()
            {
                UserId = _userDto.Id,
                IsActive = true,
                RoleId = _userDto.Role.Id
            });

        // Assert
        _mockUserRepository.Verify();
    }

    [Fact]
    public async Task UpdateEmailAsync_ShouldThrowKeyNotFoundException_WhenUserNotFound()
    {
        // Arrange
        _mockUserRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((UserDto)null)
            .Verifiable(Times.Once);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _userService.UpdateEmailAsync(
                new()
                {
                    Id = _userDto.Id,
                    Email = _userDto.Email
                }));

        _mockUserRepository.Verify();
    }

    [Fact]
    public async Task UpdateEmailAsync_ShouldThrowInvalidOperationException_WhenSameEmailInUse()
    {
        // Arrange
        _mockUserRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_userDto)
            .Verifiable(Times.Once);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _userService.UpdateEmailAsync(
                new()
                {
                    Id = _userDto.Id,
                    Email = _userDto.Email
                }));

        Assert.Equal(Messages.SameEmailInUse, exception.Message);
        _mockUserRepository.Verify();
    }

    [Fact]
    public async Task UpdateEmailAsync_ShouldThrowInvalidOperationException_WhenEmailInUse()
    {
        // Arrange
        _mockUserRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_userDto)
            .Verifiable(Times.Once);

        _mockUserRepository
            .Setup(repo => repo.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(new UserDto
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                Email = "existing@example.com"
            })
            .Verifiable(Times.Once);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _userService.UpdateEmailAsync(
                new()
                {
                    Id = _userDto.Id,
                    Email = "existing@example.com"
                }));

        Assert.Equal(Messages.EmailInUse, exception.Message);
        _mockUserRepository.Verify();
    }

    [Fact]
    public async Task UpdateEmailAsync_ShouldUpdateEmail_WhenValid()
    {
        // Arrange
        _mockUserRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_userDto)
            .Verifiable(Times.Once);

        _mockUserRepository
            .Setup(repo => repo.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((UserDto)null)
            .Verifiable(Times.Once);

        _mockUserRepository
            .Setup(repo => repo.UpdateEmailAsync(It.IsAny<UserDto>()))
            .Returns(Task.CompletedTask)
            .Verifiable(Times.Once);

        // Act
        await _userService.UpdateEmailAsync(
            new()
            {
                Id = _userDto.Id,
                Email = "test1@example.com"
            });

        // Assert
        _mockUserRepository.Verify();
    }

    [Fact]
    public async Task UpdatePasswordAsync_ShouldThrowKeyNotFoundException_WhenUserNotFound()
    {
        // Arrange
        _mockUserRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((UserDto)null)
            .Verifiable(Times.Once);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _userService.UpdatePasswordAsync(
                new()
                {
                    Id = _userDto.Id,
                    Password = _userDto.Password
                }));

        _mockUserRepository.Verify();
    }

    [Fact]
    public async Task UpdatePasswordAsync_ShouldUpdatePassword_WhenUserExists()
    {
        // Arrange
        _mockUserRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_userDto)
            .Verifiable(Times.Once);

        _mockUserRepository
            .Setup(repo => repo.UpdatePasswordAsync(It.IsAny<UserDto>()))
            .Returns(Task.CompletedTask)
            .Verifiable(Times.Once);

        // Act
        await _userService.UpdatePasswordAsync(
            new()
            {
                Id = _userDto.Id,
                Password = "newpassword"
            });

        // Assert
        _mockUserRepository.Verify();
    }

    [Fact]
    public async Task UpdateProfileAsync_ShouldThrowKeyNotFoundException_WhenUserNotFound()
    {
        // Arrange
        _mockUserRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((UserDto)null)
            .Verifiable(Times.Once);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _userService.UpdateProfileAsync(
                new()
                {
                    Id = _userDto.Id,
                    FirstName = "John",
                    LastName = "Doe",
                    DateOfBirth = new DateOnly(1990, 1, 1),
                    ContactNumber = 123456789
                }));

        _mockUserRepository.Verify();
    }

    [Fact]
    public async Task UpdateProfileAsync_ShouldUpdateProfile_WhenUserExists()
    {
        // Arrange
        _mockUserRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_userDto)
            .Verifiable(Times.Once);

        _mockUserRepository
            .Setup(repo => repo.UpdateProfileAsync(It.IsAny<UserDto>()))
            .Returns(Task.CompletedTask)
            .Verifiable(Times.Once);

        // Act
        await _userService.UpdateProfileAsync(
            new()
            {
                Id = _userDto.Id,
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = new DateOnly(1990, 1, 1),
                ContactNumber = 123456789
            });

        // Assert
        _mockUserRepository.Verify();
    }
}