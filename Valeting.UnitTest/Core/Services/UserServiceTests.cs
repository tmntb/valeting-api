using Microsoft.Extensions.Configuration;
using Moq;
using Valeting.Common.Messages;
using Valeting.Common.Models.User;
using Valeting.Core.Services;
using Valeting.Repository.Interfaces;

namespace Valeting.Tests.Core.Services;

public class UserServiceTests
{
    private readonly Guid _mockId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IConfiguration> _mockConfiguration;

    private readonly UserService _userService;

    public UserServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockConfiguration = new Mock<IConfiguration>();

        _userService = new UserService(_mockUserRepository.Object, _mockConfiguration.Object);
    }

    [Fact]
    public async Task ValidateLoginAsync_ShouldThrowKeyNotFoundException_WhenUserNotFound()
    {
        // Arrange
        _mockUserRepository.Setup(repo => repo.GetUserByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((UserDto)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _userService.ValidateLoginAsync(
                new()
                {
                    Username = "user@example.com",
                    Password = "password123"
                }));
    }

    [Fact]
    public async Task ValidateLoginAsync_ShouldReturnValid_WhenPasswordMatches()
    {
        // Arrange
        _mockUserRepository.Setup(repo => repo.GetUserByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(
                new UserDto
                {
                    Id = _mockId,
                    Username = "user@example.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("password123")
                });

        // Act
        var response = await _userService.ValidateLoginAsync(
            new()
            {
                Username = "user@example.com",
                Password = "password123"
            });

        // Assert
        Assert.True(response.Valid);
    }

    [Fact]
    public async Task ValidateLoginAsync_ShouldReturnInValid_WhenPasswordDoesNotMatches()
    {
        // Arrange
        _mockUserRepository.Setup(repo => repo.GetUserByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(
                new UserDto
                {
                    Id = _mockId,
                    Username = "user@example.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("password123")
                });

        // Act
        var response = await _userService.ValidateLoginAsync(
            new()
            {
                Username = "user@example.com",
                Password = "password"
            });

        // Assert
        Assert.False(response.Valid);
    }

    [Fact]
    public async Task GenerateTokenJWTAsync_ShouldThrowKeyNotFoundException_WhenUserNotFound()
    {
        // Arrange
        _mockUserRepository.Setup(repo => repo.GetUserByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((UserDto)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _userService.GenerateTokenJWTAsync(
                new()
                {
                    Username = "user@example.com"
                }));
    }

    [Fact]
    public async Task GenerateTokenJWTAsync_ShouldReturnValidToken_WhenUserExists()
    {
        // Arrange
        _mockUserRepository.Setup(repo => repo.GetUserByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(new UserDto
            {
                Id = _mockId,
                Username = "user@example.com"
            });

        _mockConfiguration.Setup(config => config["Jwt:Key"])
            .Returns("this_is_a_secret_key_with_128bits");
        _mockConfiguration.Setup(config => config["Jwt:Issuer"])
            .Returns("issuer");
        _mockConfiguration.Setup(config => config["Jwt:Audience"])
            .Returns("audience");

        // Act
        var response = await _userService.GenerateTokenJWTAsync(
            new()
            {
                Username = "user@example.com"
            });

        // Assert
        Assert.NotNull(response.Token);
        Assert.Equal("JwtSecurityToken", response.TokenType);
        Assert.True(response.ExpiryDate > DateTime.Now);
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrowInvalidOperationException_WhenUserExists()
    {
        // Arrange
        _mockUserRepository.Setup(repo => repo.GetUserByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(new UserDto
            {
                Id = _mockId,
                Username = "user@example.com"
            });

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _userService.RegisterAsync(
                new()
                {
                    Username = "user@example.com",
                    Password = "password"
                }));

        Assert.Equal(exception.Message, Messages.UsernameInUse);
    }

    [Fact]
    public async Task RegisterAsync_ShouldCreateUser_WhenValid()
    {
        // Arrange
        _mockUserRepository.Setup(repo => repo.GetUserByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((UserDto)null);

        // Act
        await _userService.RegisterAsync(new()
        {
            Username = "user@example.com",
            Password = "password"
        });

        // Assert
        _mockUserRepository.Verify(repo => repo.RegisterAsync(It.IsAny<UserDto>()), Times.Once);
    }
}