using Moq;
using Microsoft.Extensions.Configuration;
using Valeting.Repositories.Interfaces;
using Valeting.Services;
using Valeting.Repository.Models.User;
using System.Net;
using Valeting.Services.Objects.User;

namespace Valeting.UnitTest.Service;

public class AuthenticationServiceTests
{
    [Fact]
    public async Task GenerateToken_ValidUser_ReturnsTokenAsync()
    {
        // Arrange
        var configurationMock = new Mock<IConfiguration>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var userService = new UserService(userRepositoryMock.Object, configurationMock.Object);

        var generateTokenJWTSVRequest = new GenerateTokenJWTSVRequest { Username = "validuser@example.com" };
        var userDTO_DB = new UserDTO { Username = "validuser@example.com" };
        userRepositoryMock.Setup(r => r.FindUserByEmail(generateTokenJWTSVRequest.Username)).ReturnsAsync(userDTO_DB);
        configurationMock.Setup(x => x["Jwt:Key"]).Returns("1286c0fda9b54339fa5b25d0e311e0a21c6a9d21f45a698477e760aa9ce1aacf");
        configurationMock.Setup(x => x["Jwt:Issuer"]).Returns("TestIssuer");
        configurationMock.Setup(x => x["Jwt:Audience"]).Returns("TestAudience");

        // Act
        var result = await userService.GenerateTokenJWT(generateTokenJWTSVRequest);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Token);
        Assert.Null(result.Error);
    }

    [Fact]
    public async Task GenerateToken_UserNotFound_ReturnsError()
    {
        // Arrange
        var configurationMock = new Mock<IConfiguration>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var userService = new UserService(userRepositoryMock.Object, configurationMock.Object);

        var generateTokenJWTSVRequest = new GenerateTokenJWTSVRequest { Username = "nonexistent@example.com" };
        userRepositoryMock.Setup(r => r.FindUserByEmail(generateTokenJWTSVRequest.Username)).ReturnsAsync((UserDTO)null);

        // Act
        var result = await userService.GenerateTokenJWT(generateTokenJWTSVRequest);

        // Assert
        Assert.Null(result.Token);
        Assert.NotNull(result.Error);
        Assert.Equal((int)HttpStatusCode.NotFound, result.Error.ErrorCode);
    }
}