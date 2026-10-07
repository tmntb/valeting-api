using System.Net;
using Api.Controllers;
using Api.Models.Auth.Payload;
using Common.Messages;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Service.Interfaces;
using Service.Models.Auth.Payload;
using Service.Models.User;

namespace Api.Tests.Controllers;

public class AuthControllerTests
{
    private readonly ClaimsFixture _claimsFixture = new();
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly AuthController _authController;

    public AuthControllerTests()
    {
        _mockAuthService = new Mock<IAuthService>();

        _authController = new AuthController(_mockAuthService.Object);
    }

    [Fact]
    public async Task ForgotPassword_ShouldThrowArgumentNullException_WhenParamsAreNull()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _authController.ForgotPasswordAsync(null));
        Assert.Contains(Messages.InvalidRequestBody, exception.Message);
    }

    [Fact]
    public async Task ForgotPassword_ShouldReturnOk_WhenCredentialsAreValid()
    {
        // Arrange
        _mockAuthService
            .Setup(s => s.ForgotPasswordAsync(It.IsAny<ForgotPasswordDtoRequest>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _authController.ForgotPasswordAsync
        (
            new()
            {
                Email = "test@example.com",
                MfaCode = "123456",
                NewPassword = "newPassword"
            }
        ) as StatusCodeResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
    }


    [Fact]
    public async Task Login_ShouldThrowArgumentNullException_WhenParamsAreNull()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _authController.LoginAsync(null));
        Assert.Contains(Messages.InvalidRequestBody, exception.Message);
    }

    [Fact]
    public async Task Login_ShouldReturnOk_WhenCredentialsAreValid()
    {
        // Arrange
        _mockAuthService
            .Setup(s => s.ValidateLoginAsync(It.IsAny<UserDto>()))
            .Returns(Task.CompletedTask);

        var expiryDate = DateTime.UtcNow;
        _mockAuthService
            .Setup(s => s.GenerateTokenJWTAsync(It.IsAny<string>()))
            .ReturnsAsync(
                new GenerateTokenJWTDtoResponse
                {
                    Token = "validToken",
                    TokenType = "jwt",
                    ExpiryDate = expiryDate
                });

        // Act
        var result = await _authController.LoginAsync
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
    public async Task MfaEnableAsync_ShouldThrowArgumentNullException_WhenParamsAreNull()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _authController.MfaEnableAsync(null));
        Assert.Contains(Messages.InvalidRequestBody, exception.Message);
    }

    [Fact]
    public async Task MfaEnableAsync_ShouldReturnOk_WhenSuccessful()
    {
        // Arrange
        _claimsFixture.SetupUserClaims(_authController, Guid.Parse("00000000-0000-0000-0000-000000000001"));

        _mockAuthService
            .Setup(s => s.MfaEnableAsync(It.IsAny<MfaCodeDtoRequest>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _authController.MfaEnableAsync
        (
            new()
            {
                MfaCode = "123456"
            }
        ) as StatusCodeResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.NoContent, result.StatusCode);
    }

    [Fact]
    public async Task RecoveryCodesGenerateAsync_ShouldReturnOk_WhenSuccessful()
    {
        // Arrange
        _claimsFixture.SetupUserClaims(_authController, Guid.Parse("00000000-0000-0000-0000-000000000001"));

        _mockAuthService
            .Setup(s => s.RecoveryCodesGenerateAsync(It.IsAny<Guid>()))
            .ReturnsAsync(
                [
                    "123"
                ]
            );

        // Act
        var result = await _authController.RecoveryCodesGenerateAsync() as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
    }

    [Fact]
    public async Task MfaSetupAsync_ShouldReturnOk_WhenSuccessful()
    {
        // Arrange
        _claimsFixture.SetupUserClaims(_authController, Guid.Parse("00000000-0000-0000-0000-000000000001"));
        
        _mockAuthService
            .Setup(s => s.MfaSetupAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new MfaSetupDtoResponse
            {
                MfaQrCodeUri = ""
            });

        // Act
        var result = await _authController.MfaSetupAsync() as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
    }

    [Fact]
    public async Task RefreshTokenAsync_ShouldThrowArgumentNullException_WhenParamsAreNull()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _authController.RefreshTokenAsync(null));
        Assert.Contains(Messages.InvalidRequestBody, exception.Message);
    }

    [Fact]
    public async Task RefreshTokenAsync_ShouldThrowArgumentNullException_WhenTokenIsNull()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _authController.RefreshTokenAsync(new() { Token = null }));
        Assert.Contains(Messages.InvalidRequestBody, exception.Message);
    }

    [Fact]
    public async Task RefreshTokenAsync_ShouldReturnOk_WhenSuccessful()
    {
        // Arrange
        _mockAuthService
            .Setup(s => s.ValidateToken(It.IsAny<string>()))
            .Returns("test@example.com");

        _mockAuthService
            .Setup(s => s.GenerateTokenJWTAsync(It.IsAny<string>()))
            .ReturnsAsync(
                new GenerateTokenJWTDtoResponse
                {
                    Token = "newValidToken",
                    TokenType = "jwt",
                    ExpiryDate = DateTime.UtcNow.AddHours(1)
                });

        // Act
        var result = await _authController.RefreshTokenAsync
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
    public async Task RegisterAsync_ShouldThrowArgumentNullException_WhenParamsAreNull()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _authController.RegisterAsync(null));
        Assert.Contains(Messages.InvalidRequestBody, exception.Message);
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnCreated_WhenSuccessful()
    {
        // Arrange
        _mockAuthService
            .Setup(s => s.RegisterAsync(It.IsAny<UserDto>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _authController.RegisterAsync
        (
            new()
            {
                Email = "test@example.com",
                Password = "password",
                FirstName = "John",
                LastName = "Doe",
                ContactNumber = 123456789,
                DateOfBirth = DateOnly.FromDateTime(new DateTime(1987, 1, 12))
            }
        ) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.Created, result.StatusCode);
    }
}
