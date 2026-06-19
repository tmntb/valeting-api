using System.Net;
using Api.Controllers;
using Common.Messages;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Service.Interfaces;
using Service.Models.Auth.Payload;
using Service.Models.User;

namespace Api.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly AuthController _authController;

    public AuthControllerTests()
    {
        _mockAuthService = new Mock<IAuthService>();

        _authController = new AuthController(_mockAuthService.Object);
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
        _mockAuthService
            .Setup(s => s.MfaEnableAsync(It.IsAny<MfaEnableDtoRequest>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _authController.MfaEnableAsync
        (
            new()
            {
                Email = "user@example.com",
                MfaCode = "123456"
            }
        ) as StatusCodeResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.NoContent, result.StatusCode);
    }

    [Fact]
    public async Task MfaSetupAsync_ShouldThrowArgumentNullException_WhenParamsAreNull()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _authController.MfaSetupAsync(null));
        Assert.Contains(Messages.InvalidRequestBody, exception.Message);
    }

    [Fact]
    public async Task MfaSetupAsync_ShouldReturnOk_WhenSuccessful()
    {
        // Arrange
        _mockAuthService
            .Setup(s => s.MfaSetupAsync(It.IsAny<string>()))
            .ReturnsAsync(new MfaSetupDtoResponse
            {
                MfaQrCodeUri = "",
                RecoveryCodes = []
            });

        // Act
        var result = await _authController.MfaSetupAsync
        (
            new()
            {
                Email = "user@example.com"
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
