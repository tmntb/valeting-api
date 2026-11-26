using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _tokenService.GenerateTokenJwtAsync(new()
        {
            Email = "user@example.com"
        }));
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
        var response = await _tokenService.GenerateTokenJwtAsync(new()
        {
            Email = "user@example.com"
        });

        // Assert
        Assert.NotNull(response.Token);
        Assert.Equal("JwtSecurityToken", response.TokenType);
        Assert.True(response.ExpiryDate > DateTime.Now);
        Assert.Equal(_userDto.Role.Code, response.Role);
    }

    [Fact]
    public async Task GenerateTokenJWTAsync_ShouldUseCustomLifetimeAndClaims()
    {
        _mockUserRepository
            .Setup(repo => repo.GetByEmailAsync("user@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(_userDto);
        _mockConfiguration.Setup(config => config["Jwt:Key"])
            .Returns("this_is_a_secret_key_with_128bits");
        _mockConfiguration.Setup(config => config["Jwt:Issuer"]).Returns("issuer");
        _mockConfiguration.Setup(config => config["Jwt:Audience"]).Returns("audience");

        var response = await _tokenService.GenerateTokenJwtAsync(new()
        {
            Email = "user@example.com",
            Ttl = 15,
            Claims =
            [
                new Claim("purpose", "password-reset"),
                new Claim(JwtRegisteredClaimNames.Jti, "reset-jti")
            ]
        });

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(response.Token);
        Assert.Contains(jwt.Claims, claim => claim.Type == "purpose" && claim.Value == "password-reset");
        Assert.Contains(jwt.Claims, claim => claim.Type == JwtRegisteredClaimNames.Jti && claim.Value == "reset-jti");
        Assert.True(jwt.ValidTo > DateTime.UtcNow.AddMinutes(14));
        Assert.True(jwt.ValidTo <= DateTime.UtcNow.AddMinutes(16));
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
            .Setup(repo => repo.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_userDto);

        var tokenResponse = await _tokenService.GenerateTokenJwtAsync(new()
        {
            Email = "user@example.com"
        });
        var validToken = tokenResponse.Token;

        // Act
        _tokenService.ValidateTokenJwt(validToken);

        // Assert
        _mockUserRepository.Verify(repo => repo.GetByEmailAsync("user@example.com", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ValidateToken_ShouldThrowUnauthorized_WhenTokenIsInvalid()
    {
        _mockConfiguration.Setup(config => config["Jwt:Key"])
            .Returns("this_is_a_secret_key_with_128bits");
        _mockConfiguration.Setup(config => config["Jwt:Issuer"]).Returns("issuer");
        _mockConfiguration.Setup(config => config["Jwt:Audience"]).Returns("audience");

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            Task.Run(() => _tokenService.ValidateTokenJwt("not-a-jwt")));
    }
}
