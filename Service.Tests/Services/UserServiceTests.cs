using Common.Enums;
using Common.Messages;
using Microsoft.Extensions.Configuration;
using Moq;
using Service.Interfaces;
using Service.Models.Role;
using Service.Models.User;
using Service.Services;

namespace Service.Tests.Services;

public class UserServiceTests
{
    private readonly Guid _mockId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IRoleRepository> _mockRoleRepository;
    private readonly Mock<IConfiguration> _mockConfiguration;

    private readonly UserService _userService;

    public UserServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockConfiguration = new Mock<IConfiguration>();
        _mockRoleRepository = new Mock<IRoleRepository>();

        _userService = new UserService(_mockUserRepository.Object, _mockRoleRepository.Object, _mockConfiguration.Object);
    }

    [Fact]
    public async Task GenerateTokenJWTAsync_ShouldThrowKeyNotFoundException_WhenUserNotFound()
    {
        // Arrange
        _mockUserRepository.Setup(repo => repo.GetUserByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((UserDto)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _userService.GenerateTokenJWTAsync("user@example.com"));
    }

    [Fact]
    public async Task GenerateTokenJWTAsync_ShouldReturnValidToken_WhenUserExists()
    {
        // Arrange
        _mockUserRepository.Setup(repo => repo.GetUserByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(new UserDto
            {
                Id = _mockId,
                Username = "username",
                Email = "user@example.com",
                Role = new RoleDto
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                    Name = RoleEnum.User
                }
            });

        _mockConfiguration.Setup(config => config["Jwt:Key"])
            .Returns("this_is_a_secret_key_with_128bits");
        _mockConfiguration.Setup(config => config["Jwt:Issuer"])
            .Returns("issuer");
        _mockConfiguration.Setup(config => config["Jwt:Audience"])
            .Returns("audience");

        // Act
        var response = await _userService.GenerateTokenJWTAsync("user@example.com");

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

                Email = "user@example.com"
            });

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _userService.RegisterAsync(
                new()
                {
                    Username = "username",
                    Password = "password",
                    Email = "user@example.com",
                    ContactNumber = 123456789,
                    RoleName = RoleEnum.User
                }));

        Assert.Equal(exception.Message, Messages.UsernameInUse);
    }

    [Fact]
    public async Task RegisterAsync_ShouldCreateUser_WhenValid()
    {
        // Arrange
        _mockUserRepository.Setup(repo => repo.GetUserByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((UserDto)null);

        _mockRoleRepository.Setup(repo => repo.GetByNameAsync(It.IsAny<RoleEnum>()))
            .ReturnsAsync(new RoleDto
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                Name = RoleEnum.User
            });

        // Act
        await _userService.RegisterAsync(new()
        {
            Username = "username",
            Password = "password",
            Email = "user@example.com",
            ContactNumber = 123456789,
            RoleName = RoleEnum.User
        });

        // Assert
        _mockUserRepository.Verify(repo => repo.RegisterAsync(It.IsAny<UserDto>()), Times.Once);
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
                    Email = "user@example.com",
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
                    Email = "user@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                    IsActive = true
                });

        // Act
        var response = await _userService.ValidateLoginAsync(
            new()
            {
                Email = "user@example.com",
                Password = "password123"
            });

        // Assert
        Assert.True(response);
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
                    Email = "user@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123")
                });

        // Act
        var response = await _userService.ValidateLoginAsync(
            new()
            {
                Email = "user@example.com",
                Password = "password"
            });

        // Assert
        Assert.False(response);
    }

    [Fact]
    public async Task ValidateToken_ShouldReturnEmail_WhenTokenIsValid()
    {
        // Arrange
        _mockConfiguration.Setup(config => config["Jwt:Key"])
            .Returns("this_is_a_secret_key_with_128bits");
        _mockConfiguration.Setup(config => config["Jwt:Issuer"])
            .Returns("issuer");
        _mockConfiguration.Setup(config => config["Jwt:Audience"])
            .Returns("audience");

        _mockUserRepository.Setup(repo => repo.GetUserByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(new UserDto
            {
                Id = _mockId,
                Username = "username",
                Email = "user@example.com",
                Role = new RoleDto
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                    Name = RoleEnum.User
                }
            });

        var tokenResponse = await _userService.GenerateTokenJWTAsync("user@example.com");
        var validToken = tokenResponse.Token;

        // Act
        var email = _userService.ValidateToken(validToken);

        // Assert
        Assert.Equal("user@example.com", email);
    }
}