using Api.Controllers;
using Api.Models.User.Payload;
using Common.Messages;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Service.Interfaces;
using Service.Models.User;
using Service.Models.User.Payload;
using System.Net;

namespace Api.Tests.Controllers;

public class UserControllerTests
{
    private readonly ClaimsFixture _claimsFixture = new();
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
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _userController.LoginAsync(null));
        Assert.Contains(Messages.InvalidRequestBody, exception.Message);
    }

    [Fact]
    public async Task Login_ShouldReturnOk_WhenCredentialsAreValid()
    {
        // Arrange
        _mockUserService
            .Setup(s => s.ValidateLoginAsync(It.IsAny<UserDto>()))
            .Returns(Task.CompletedTask);

        var expiryDate = DateTime.UtcNow;
        _mockUserService
            .Setup(s => s.GenerateTokenJWTAsync(It.IsAny<string>()))
            .ReturnsAsync(
                new GenerateTokenJWTDtoResponse
                {
                    Token = "validToken",
                    TokenType = "jwt",
                    ExpiryDate = expiryDate
                });

        // Act
        var result = await _userController.LoginAsync
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
        _mockUserService
            .Setup(s => s.ValidateToken(It.IsAny<string>()))
            .Returns("test@example.com");

        _mockUserService
            .Setup(s => s.GenerateTokenJWTAsync(It.IsAny<string>()))
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
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _userController.RegisterAsync(null));
        Assert.Contains(Messages.InvalidRequestBody, exception.Message);
    }

    [Fact]
    public async Task Register_ShouldReturnOk_WhenSuccessful()
    {
        // Arrange
        _mockUserService
            .Setup(s => s.RegisterAsync(It.IsAny<UserDto>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _userController.RegisterAsync
        (
            new()
            {
                Password = "password"
            }
        ) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.Created, result.StatusCode);
    }

    [Fact]
    public async Task Reset_ShouldThrowArgumentNullException_WhenParamsAreNull()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _userController.ResetAsync(null));
        Assert.Contains(Messages.InvalidRequestBody, exception.Message);
    }

    [Fact]
    public async Task Reset_ShouldReturnNoContent_WhenSuccessful()
    {
        // Arrange
        _mockUserService
            .Setup(s => s.ResetAsync(It.IsAny<UserDto>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _userController.ResetAsync
        (
            new()
            {
                Email = "user@example.com",
                NewPassword = "newpassword"
            }
        ) as NoContentResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.NoContent, result.StatusCode);
    }

    [Fact]
    public async Task UpdateAdminSettingsAsync_ShouldThrowArgumentNullException_WhenParamsAreNull()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _userController.UpdateAdminSettingsAsync(null));
        Assert.Contains(Messages.InvalidRequestBody, exception.Message);
    }

    [Fact]
    public async Task UpdateAdminSettingsAsync_ShouldReturnNoContent_WhenSuccessful()
    {
        // Arrange
        _claimsFixture.SetupUserClaims(_userController, Guid.Parse("00000000-0000-0000-0000-000000000001"));

        _mockUserService
            .Setup(s => s.UpdateAdminSettingsAsync(It.IsAny<UpdateAdminSettingsDtoRequest>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _userController.UpdateAdminSettingsAsync
        (
            new()
            {
                UserId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                IsActive = true,
                RoleId = Guid.Parse("00000000-0000-0000-0000-000000000002")
            }
        ) as NoContentResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.NoContent, result.StatusCode);
    }

    [Fact]
    public async Task UpdateEmailAsync_ShouldThrowArgumentNullException_WhenParamsAreNull()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _userController.UpdateEmailAsync(null));
        Assert.Contains(Messages.InvalidRequestBody, exception.Message);
    }

    [Fact]
    public async Task UpdateEmailAsync_ShouldReturnNoContent_WhenSuccessful()
    {
        // Arrange
        _claimsFixture.SetupUserClaims(_userController, Guid.Parse("00000000-0000-0000-0000-000000000001"));

        _mockUserService
            .Setup(s => s.UpdateEmailAsync(It.IsAny<UserDto>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _userController.UpdateEmailAsync
        (
            new()
            {
                Email = "test@example.com"
            }
        ) as NoContentResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.NoContent, result.StatusCode);
    }

    [Fact]
    public async Task UpdatePasswordAsync_ShouldThrowArgumentNullException_WhenParamsAreNull()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _userController.UpdatePasswordAsync(null));
        Assert.Contains(Messages.InvalidRequestBody, exception.Message);
    }

    [Fact]
    public async Task UpdatePasswordAsync_ShouldReturnNoContent_WhenSuccessful()
    {
        // Arrange
        _claimsFixture.SetupUserClaims(_userController, Guid.Parse("00000000-0000-0000-0000-000000000001"));

        _mockUserService
            .Setup(s => s.UpdatePasswordAsync(It.IsAny<UserDto>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _userController.UpdatePasswordAsync
        (
            new()
            {
                Password = "1234"
            }
        ) as NoContentResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.NoContent, result.StatusCode);
    }

    [Fact]
    public async Task UpdateProfileAsync_ShouldThrowArgumentNullException_WhenParamsAreNull()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _userController.UpdateProfileAsync(null));
        Assert.Contains(Messages.InvalidRequestBody, exception.Message);
    }

    [Fact]
    public async Task UpdateProfileAsync_ShouldReturnNoContent_WhenSuccessful()
    {
        // Arrange
        _claimsFixture.SetupUserClaims(_userController, Guid.Parse("00000000-0000-0000-0000-000000000001"));

        _mockUserService
            .Setup(s => s.UpdateProfileAsync(It.IsAny<UpdateProfileDtoRequest>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _userController.UpdateProfileAsync
        (
            new()
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = DateOnly.FromDateTime(DateTime.Parse("1990-01-01")),
                ContactNumber = 123456789
            }
        ) as NoContentResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.NoContent, result.StatusCode);
    }
}
