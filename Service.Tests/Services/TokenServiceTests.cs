using Microsoft.Extensions.Configuration;
using Moq;
using Service.Interfaces;
using Service.Models.User;
using Service.Services;

namespace Service.Tests.Services;

public class TokenServiceTests
{
    private readonly UserDto _userDto;

    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IConfiguration> _mockConfiguration;

    private readonly TokenService _tokenService;

    public TokenServiceTests()
    {
        _userDto = DataFactory.CreateUserDto();
        
        _mockUserRepository = new Mock<IUserRepository>();
        _mockConfiguration = new Mock<IConfiguration>();

        _tokenService = new TokenService(_mockUserRepository.Object, _mockConfiguration.Object);
    }

    [Fact]
    public async Task GenerateTokenJWTAsync_ShouldThrowKeyNotFoundException_WhenUserNotFound()
    {
        // Arrange
        _mockUserRepository
            .Setup(repo => repo.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((UserDto)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _tokenService.GenerateTokenJWTAsync("user@example.com"));
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
        var response = await _tokenService.GenerateTokenJWTAsync("user@example.com");

        // Assert
        Assert.NotNull(response.Token);
        Assert.Equal("JwtSecurityToken", response.TokenType);
        Assert.True(response.ExpiryDate > DateTime.Now);
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

        var tokenResponse = await _tokenService.GenerateTokenJWTAsync("user@example.com");
        var validToken = tokenResponse.Token;

        // Act
        var email = _tokenService.ValidateToken(validToken);

        // Assert
        Assert.Equal("user@example.com", email);
    }
}
