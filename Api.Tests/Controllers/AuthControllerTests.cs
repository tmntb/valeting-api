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
    private readonly ClaimsFixture _claimsFixture = new();
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
    public async Task MfaRegenerateRecoveryCodesAsync_ShouldThrowArgumentNullException_WhenParamsAreNull()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _authController.MfaRegenerateRecoveryCodesAsync(null));
        Assert.Contains(Messages.InvalidRequestBody, exception.Message);
    }

    [Fact]
    public async Task MfaRegenerateRecoveryCodesAsync_ShouldReturnOk_WhenSuccessful()
    {
        // Arrange
        _claimsFixture.SetupUserClaims(_authController, Guid.Parse("00000000-0000-0000-0000-000000000001"));

        _mockAuthService
            .Setup(s => s.MfaRegenerateRecoveryCodesAsync(It.IsAny<MfaCodeDtoRequest>()))
            .ReturnsAsync(
                [
                    "123"
                ]
            );

        // Act
        var result = await _authController.MfaRegenerateRecoveryCodesAsync
        (
            new()
            {
                MfaCode = "123456"
            }
        ) as ObjectResult;

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
                MfaQrCodeUri = "",
                RecoveryCodes = []
            });

        // Act
        var result = await _authController.MfaSetupAsync() as ObjectResult;

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
