using Api.Controllers;
using Api.Models.User.Payload;
using Common.Messages;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Service.Interfaces;
using Service.Models.User.Payload;
using System.Net;

namespace Api.Tests.Controllers;

public class UserControllerTests
{
    private readonly Mock<IUserService> _mockUserService;

    private readonly UserController _userController;

    public UserControllerTests()
    {
        _mockUserService = new Mock<IUserService>();

        _userController = new UserController(_mockUserService.Object);
    }

    [Fact]
    public async Task Login_ShouldThrowArgumentNullException_WhenParamsAreNull()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _userController.Login(null));
        Assert.Contains(Messages.InvalidRequestBody, exception.Message);
    }

    [Fact]
    public async Task Login_ShouldThrowUnauthorizedAccessException_WhenInvalidCredentials()
    {
        // Arrange
        _mockUserService.Setup(s => s.ValidateLoginAsync(It.IsAny<ValidateLoginDtoRequest>()))
            .ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _userController.Login(
            new()
            {
                Email = "test@example.com",
                Password = "wrongpassword"
            }));

        Assert.Equal(Messages.InvalidPassword, exception.Message);
    }

    [Fact]
    public async Task Login_ShouldReturnOk_WhenCredentialsAreValid()
    {
        // Arrange
        _mockUserService.Setup(s => s.ValidateLoginAsync(It.IsAny<ValidateLoginDtoRequest>()))
            .ReturnsAsync(true);

        var expiryDate = DateTime.UtcNow;
        _mockUserService.Setup(s => s.GenerateTokenJWTAsync(It.IsAny<string>()))
            .ReturnsAsync(
                new GenerateTokenJWTDtoResponse
                {
                    Token = "validToken",
                    TokenType = "jwt",
                    ExpiryDate = expiryDate
                });

        // Act
        var result = await _userController.Login
        (
            new()
            {
                Email = "test@example.com",
                Password = "password"
            }
        ) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);

        var responseApi = (LoginApiResponse)result.Value;
        Assert.Equal("validToken", responseApi.Token);
        Assert.Equal(expiryDate, responseApi.ExpiryDate);
        Assert.Equal("jwt", responseApi.TokenType);
    }

    [Fact]
    public async Task RefreshTokenAsync_ShouldThrowArgumentNullException_WhenParamsAreNull()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _userController.RefreshTokenAsync(null));
        Assert.Contains(Messages.InvalidRequestBody, exception.Message);
    }

    [Fact]
    public async Task RefreshTokenAsync_ShouldThrowArgumentNullException_WhenTokenIsNull()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _userController.RefreshTokenAsync(new() { Token = null }));
        Assert.Contains(Messages.InvalidRequestBody, exception.Message);
    }

    [Fact]
    public async Task RefreshTokenAsync_ShouldReturnOk_WhenSuccessful()
    {
        // Arrange
        _mockUserService.Setup(s => s.ValidateToken(It.IsAny<string>()))
            .Returns("test@example.com");

        _mockUserService.Setup(s => s.GenerateTokenJWTAsync(It.IsAny<string>()))
            .ReturnsAsync(
                new GenerateTokenJWTDtoResponse
                {
                    Token = "newValidToken",
                    TokenType = "jwt",
                    ExpiryDate = DateTime.UtcNow.AddHours(1)
                });

        // Act
        var result = await _userController.RefreshTokenAsync
        (
            new()
            {
                Token = "oldValidToken"
            }
        ) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
    }

    [Fact]
    public async Task Register_ShouldThrowArgumentNullException_WhenParamsAreNull()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _userController.Register(null));
        Assert.Contains(Messages.InvalidRequestBody, exception.Message);
    }

    [Fact]
    public async Task Register_ShouldReturnOk_WhenSuccessful()
    {
        // Arrange
        _mockUserService.Setup(s => s.RegisterAsync(It.IsAny<RegisterDtoRequest>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _userController.Register
        (
            new()
            {
                Username = "test@example.com",
                Password = "password"
            }
        ) as StatusCodeResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.Created, result.StatusCode);
    }
}
