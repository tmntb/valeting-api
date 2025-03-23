using Moq;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Net;
using System.Text;
using Valeting.Core.Services;
using Valeting.Common.Messages;
using Valeting.Common.Models.User;
using Valeting.Repository.Interfaces;

namespace Valeting.Tests.Core;

public class UserServiceTests
{
    //private readonly Mock<IUserRepository> _mockUserRepository;
    //private readonly Mock<IConfiguration> _mockConfiguration;
    //private readonly UserService _userService;

    //public UserServiceTests()
    //{
    //    _mockUserRepository = new Mock<IUserRepository>();
    //    _mockConfiguration = new Mock<IConfiguration>();
    //    _userService = new UserService(_mockUserRepository.Object, _mockConfiguration.Object);
    //}

    //[Fact]
    //public async Task ValidateLoginAsync_ShouldReturnBadRequest_WhenValidationFails()
    //{
    //    // Arrange
    //    var request = new ValidateLoginDtoRequest { Username = "invalid", Password = null };
        
    //    // Act
    //    var response = await _userService.ValidateLoginAsync(request);

    //    // Assert
    //    Assert.False(response.Valid);
    //    Assert.NotNull(response.Error);
    //    Assert.Equal((int)HttpStatusCode.BadRequest, response.Error.ErrorCode);
    //}

    //[Fact]
    //public async Task ValidateLoginAsync_ShouldReturnNotFound_WhenUserNotFound()
    //{
    //    // Arrange
    //    var request = new ValidateLoginDtoRequest { Username = "user@example.com", Password = "password123" };
    //    _mockUserRepository.Setup(repo => repo.GetUserByEmailAsync(request.Username)).ReturnsAsync((UserDto)null);

    //    // Act
    //    var response = await _userService.ValidateLoginAsync(request);

    //    // Assert
    //    Assert.False(response.Valid);
    //    Assert.NotNull(response.Error);
    //    Assert.Equal((int)HttpStatusCode.NotFound, response.Error.ErrorCode);
    //    Assert.Equal(Messages.NotFound, response.Error.Message);
    //}

    //[Fact]
    //public async Task ValidateLoginAsync_ShouldReturnValid_WhenPasswordMatches()
    //{
    //    // Arrange
    //    var saltValue = "saltValue";
    //    var request = new ValidateLoginDtoRequest { Username = "user@example.com", Password =   "password123"};
    //    byte[] salt = Encoding.ASCII.GetBytes(saltValue);
    //    var hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(request.Password, salt, KeyDerivationPrf.HMACSHA256, 100000, 256 / 8));
    //    var userDto = new UserDto
    //    {
    //        Username = "user@example.com",
    //        Password = hashed
    //    };

    //    _mockUserRepository.Setup(repo => repo.GetUserByEmailAsync(request.Username)).ReturnsAsync(userDto);

    //    // Act
    //    var response = await _userService.ValidateLoginAsync(request);

    //    // Assert
    //    Assert.True(response.Valid);
    //    Assert.Null(response.Error);
    //}

    //[Fact]
    //public async Task GenerateTokenJWTAsync_ShouldReturnBadRequest_WhenValidationFails()
    //{
    //    // Arrange
    //    var request = new GenerateTokenJWTDtoRequest { Username = null };

    //    // Act
    //    var response = await _userService.GenerateTokenJWTAsync(request);

    //    // Assert
    //    Assert.Null(response.Token);
    //    Assert.NotNull(response.Error);
    //    Assert.Equal((int)HttpStatusCode.BadRequest, response.Error.ErrorCode);
    //}

    //[Fact]
    //public async Task GenerateTokenJWTAsync_ShouldReturnNotFound_WhenUserNotFound()
    //{
    //    // Arrange
    //    var request = new GenerateTokenJWTDtoRequest { Username = "user@example.com" };
    //    _mockUserRepository.Setup(repo => repo.GetUserByEmailAsync(request.Username)).ReturnsAsync((UserDto)null); // User not found

    //    // Act
    //    var response = await _userService.GenerateTokenJWTAsync(request);

    //    // Assert
    //    Assert.Null(response.Token);
    //    Assert.NotNull(response.Error);
    //    Assert.Equal((int)HttpStatusCode.NotFound, response.Error.ErrorCode);
    //}

    //[Fact]
    //public async Task GenerateTokenJWTAsync_ShouldReturnToken_WhenUserExists()
    //{
    //    // Arrange
    //    var request = new GenerateTokenJWTDtoRequest { Username = "user@example.com" };
    //    var userDto = new UserDto
    //    {
    //        Id = Guid.NewGuid(),
    //        Username = "user@example.com"
    //    };

    //    _mockUserRepository.Setup(repo => repo.GetUserByEmailAsync(request.Username)).ReturnsAsync(userDto);

    //    _mockConfiguration.Setup(config => config["Jwt:Key"]).Returns("this_is_a_secret_key_with_128bits");
    //    _mockConfiguration.Setup(config => config["Jwt:Issuer"]).Returns("issuer");
    //    _mockConfiguration.Setup(config => config["Jwt:Audience"]).Returns("audience");

    //    // Act
    //    var response = await _userService.GenerateTokenJWTAsync(request);

    //    // Assert
    //    Assert.NotNull(response.Token);
    //    Assert.Null(response.Error);
    //    Assert.Equal("JwtSecurityToken", response.TokenType);
    //    Assert.True(response.ExpiryDate > DateTime.Now);
    //}
}