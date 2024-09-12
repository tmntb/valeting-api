using Moq;
using Microsoft.Extensions.Configuration;

using System.Net;

using Valeting.Core.Services;
using Valeting.Core.Models.User;
using Valeting.Repository.Models.User;
using Valeting.Repository.Repositories.Interfaces;
using Valeting.Core.Validators;
using Valeting.Common.Messages;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Valeting.UnitTest.Core;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _configurationMock = new Mock<IConfiguration>();
        _userService = new UserService(_userRepositoryMock.Object, _configurationMock.Object);
    }

    [Fact]
    public async Task ValidateLoginAsync_ShouldReturnBadRequest_WhenValidationFails()
    {
        // Arrange
        var request = new ValidateLoginSVRequest { Username = "invalid", Password = null };
        
        // Act
        var response = await _userService.ValidateLoginAsync(request);

        // Assert
        Assert.False(response.Valid);
        Assert.NotNull(response.Error);
        Assert.Equal((int)HttpStatusCode.BadRequest, response.Error.ErrorCode);
    }

    [Fact]
    public async Task ValidateLoginAsync_ShouldReturnNotFound_WhenUserNotFound()
    {
        // Arrange
        var request = new ValidateLoginSVRequest { Username = "user@example.com", Password = "password123" };
        _userRepositoryMock.Setup(repo => repo.FindUserByEmailAsync(request.Username)).ReturnsAsync((UserDTO)null);

        // Act
        var response = await _userService.ValidateLoginAsync(request);

        // Assert
        Assert.False(response.Valid);
        Assert.NotNull(response.Error);
        Assert.Equal((int)HttpStatusCode.NotFound, response.Error.ErrorCode);
        Assert.Equal(Messages.UserNotFound, response.Error.Message);
    }

    [Fact]
    public async Task ValidateLoginAsync_ShouldReturnValid_WhenPasswordMatches()
    {
        // Arrange
        var saltValue = "saltValue";
        var request = new ValidateLoginSVRequest { Username = "user@example.com", Password =   "password123"};
        byte[] salt = Encoding.ASCII.GetBytes(saltValue);
        var hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(request.Password, salt, KeyDerivationPrf.HMACSHA256, 100000, 256 / 8));
        var userDTO = new UserDTO
        {
            Username = "user@example.com",
            Password = hashed,
            Salt = saltValue
        };

        _userRepositoryMock.Setup(repo => repo.FindUserByEmailAsync(request.Username)).ReturnsAsync(userDTO);

        // Act
        var response = await _userService.ValidateLoginAsync(request);

        // Assert
        Assert.True(response.Valid);
        Assert.Null(response.Error);
    }

    [Fact]
    public async Task GenerateTokenJWTAsync_ShouldReturnBadRequest_WhenValidationFails()
    {
        // Arrange
        var request = new GenerateTokenJWTSVRequest { Username = null };

        // Act
        var response = await _userService.GenerateTokenJWTAsync(request);

        // Assert
        Assert.Null(response.Token);
        Assert.NotNull(response.Error);
        Assert.Equal((int)HttpStatusCode.BadRequest, response.Error.ErrorCode);
    }

    [Fact]
    public async Task GenerateTokenJWTAsync_ShouldReturnNotFound_WhenUserNotFound()
    {
        // Arrange
        var request = new GenerateTokenJWTSVRequest { Username = "user@example.com" };
        _userRepositoryMock.Setup(repo => repo.FindUserByEmailAsync(request.Username)).ReturnsAsync((UserDTO)null); // User not found

        // Act
        var response = await _userService.GenerateTokenJWTAsync(request);

        // Assert
        Assert.Null(response.Token);
        Assert.NotNull(response.Error);
        Assert.Equal((int)HttpStatusCode.NotFound, response.Error.ErrorCode);
    }

    [Fact]
    public async Task GenerateTokenJWTAsync_ShouldReturnToken_WhenUserExists()
    {
        // Arrange
        var request = new GenerateTokenJWTSVRequest { Username = "user@example.com" };
        var userDTO = new UserDTO
        {
            Id = Guid.NewGuid(),
            Username = "user@example.com"
        };

        _userRepositoryMock.Setup(repo => repo.FindUserByEmailAsync(request.Username)).ReturnsAsync(userDTO);

        _configurationMock.Setup(config => config["Jwt:Key"]).Returns("this_is_a_secret_key_with_128bits");
        _configurationMock.Setup(config => config["Jwt:Issuer"]).Returns("issuer");
        _configurationMock.Setup(config => config["Jwt:Audience"]).Returns("audience");

        // Act
        var response = await _userService.GenerateTokenJWTAsync(request);

        // Assert
        Assert.NotNull(response.Token);
        Assert.Null(response.Error);
        Assert.Equal("JwtSecurityToken", response.TokenType);
        Assert.True(response.ExpiryDate > DateTime.Now);
    }
}