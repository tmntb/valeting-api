using System.Net;
using Api.Context;
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
    private readonly Mock<IRecoveryCodeService> _mockRecoveryCodeService;
    
    private readonly AuthController _authController;

    public AuthControllerTests()
    {
        _mockAuthService = new Mock<IAuthService>();
        _mockRecoveryCodeService = new Mock<IRecoveryCodeService>();
        
        var valetingApiContext = new ValetingApiContext
        {
            UserId = Guid.Parse("00000000-0000-0000-0000-000000000099")
        };

        _authController = new AuthController(valetingApiContext, _mockAuthService.Object, _mockRecoveryCodeService.Object);
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

        // Act
        var result = await _authController.LoginAsync
        (
            new()
            {
                Email = "test@example.com",
                Password = "password"
            }
        ) as StatusCodeResult;

        // Assert
        Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
    }

    [Fact]
    public async Task RecoveryCodesGenerateAsync_ShouldReturnOk_WhenSuccessful()
    {
        // Arrange
        _mockRecoveryCodeService
            .Setup(s => s.GenerateRecoveryCodesAsync(It.IsAny<Guid>()))
            .ReturnsAsync(
                [
                    "123"
                ]
            );

        // Act
        var result = await _authController.RecoveryCodesAsync() as ObjectResult;

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