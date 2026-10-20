using System.Net;
using System.Text.Json;
using Api.Controllers;
using Api.Models.Auth.Payload;
using Api.Models.WebAuthn.Payload;
using Common.Messages;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Service.Interfaces;
using Service.Models.Auth;
using Service.Models.Auth.Payload;
using Service.Models.Passkey;
using Service.Models.User;

namespace Api.Tests.Controllers;

public class AuthControllerTests
{
    private readonly ClaimsFixture _claimsFixture = new();
    private readonly Mock<IAuthService> _mockAuthService;
    // private readonly Mock<IMfaService> _mockMfaService;
    private readonly Mock<IRecoveryCodeService> _mockRecoveryCodeService;
    private readonly Mock<ITokenService> _mockTokenService;
    private readonly Mock<IWebAuthnService> _mockPasskeyService;
    private readonly AuthController _authController;

    public AuthControllerTests()
    {
        _mockAuthService = new Mock<IAuthService>();
        // _mockMfaService = new Mock<IMfaService>();
        _mockRecoveryCodeService = new Mock<IRecoveryCodeService>();
        _mockTokenService = new Mock<ITokenService>();
        _mockPasskeyService = new Mock<IWebAuthnService>();

        _authController = new AuthController(_mockAuthService.Object, _mockRecoveryCodeService.Object, _mockTokenService.Object, _mockPasskeyService.Object);
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
        _mockTokenService
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

    // [Fact]
    // public async Task MfaEnableAsync_ShouldThrowArgumentNullException_WhenParamsAreNull()
    // {
    //     // Act & Assert
    //     var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _authController.MfaVerifyAsync(null));
    //     Assert.Contains(Messages.InvalidRequestBody, exception.Message);
    // }
    //
    // [Fact]
    // public async Task MfaEnableAsync_ShouldReturnOk_WhenSuccessful()
    // {
    //     // Arrange
    //     _claimsFixture.SetupUserClaims(_authController, Guid.Parse("00000000-0000-0000-0000-000000000001"));
    //
    //     _mockMfaService
    //         .Setup(s => s.MfaVerifyAsync(It.IsAny<MfaCodeDtoRequest>()))
    //         .Returns(Task.CompletedTask);
    //
    //     // Act
    //     var result = await _authController.MfaVerifyAsync
    //     (
    //         new()
    //         {
    //             MfaCode = "123456"
    //         }
    //     ) as StatusCodeResult;
    //
    //     // Assert
    //     Assert.NotNull(result);
    //     Assert.Equal((int)HttpStatusCode.NoContent, result.StatusCode);
    // }

    [Fact]
    public async Task RecoveryCodesGenerateAsync_ShouldReturnOk_WhenSuccessful()
    {
        // Arrange
        _claimsFixture.SetupUserClaims(_authController, Guid.Parse("00000000-0000-0000-0000-000000000001"));

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

    // [Fact]
    // public async Task MfaSetupAsync_ShouldReturnOk_WhenSuccessful()
    // {
    //     // Arrange
    //     _claimsFixture.SetupUserClaims(_authController, Guid.Parse("00000000-0000-0000-0000-000000000001"));
    //     
    //     _mockMfaService
    //         .Setup(s => s.MfaSetupAsync(It.IsAny<Guid>()))
    //         .ReturnsAsync(new MfaSetupDtoResponse
    //         {
    //             MfaQrCodeUri = ""
    //         });
    //
    //     // Act
    //     var result = await _authController.MfaSetupAsync() as ObjectResult;
    //
    //     // Assert
    //     Assert.NotNull(result);
    //     Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
    // }

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
        _mockTokenService
            .Setup(s => s.ValidateToken(It.IsAny<string>()))
            .Returns("test@example.com");

        _mockTokenService
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

    // [Fact]
    // public async Task PasskeyAuthenticateOptionsAsync_ShouldReturnOk_WhenRequestIsValid()
    // {
    //     // Arrange
    //     _mockPasskeyService
    //         .Setup(s => s.CreateAuthenticationOptionsAsync(It.IsAny<string?>()))
    //         .ReturnsAsync(new WebauthnAuthenticationOptionsDtoResponse
    //         {
    //             CeremonyId = Guid.NewGuid(),
    //             PublicKey = JsonDocument.Parse("{}").RootElement.Clone()
    //         });
    //
    //     // Act
    //     var result = await _authController.PasskeyAuthenticateOptionsAsync(new WebAuthnAuthenticateOptionsApiRequest
    //     {
    //         Email = "test@example.com"
    //     }) as ObjectResult;
    //
    //     // Assert
    //     Assert.NotNull(result);
    //     Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
    // }

    // [Fact]
    // public async Task PasskeyRegisterOptionsAsync_ShouldReturnOk_WhenUserIsAuthenticated()
    // {
    //     // Arrange
    //     _claimsFixture.SetupUserClaims(_authController, Guid.Parse("00000000-0000-0000-0000-000000000001"));

    //     _mockPasskeyService
    //         .Setup(s => s.CreateRegistrationOptionsAsync(It.IsAny<Guid>()))
    //         .ReturnsAsync(new PasskeyRegistrationOptionsResponse
    //         {
    //             CeremonyId = Guid.NewGuid(),
    //             PublicKey = JsonDocument.Parse("{}").RootElement.Clone()
    //         });

    //     // Act
    //     var result = await _authController.PasskeyRegisterOptionsAsync() as ObjectResult;

    //     // Assert
    //     Assert.NotNull(result);
    //     Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
    // }

    [Fact]
    public async Task PasskeyAuthenticateVerifyAsync_ShouldNotIssueToken_WhenVerificationFails()
    {
        _mockPasskeyService
            .Setup(s => s.VerifyAuthenticationAsync(It.IsAny<PasskeyAuthenticationVerifyRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedAccessException());

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _authController.PasskeyAuthenticateVerifyAsync(new()
        {
            CeremonyId = Guid.NewGuid(),
            Credential = null!
        }));

        _mockTokenService.Verify(s => s.GenerateTokenJWTAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task PasskeysAsync_ShouldReturnOnlyPublicCredentialMetadata()
    {
        _claimsFixture.SetupUserClaims(_authController, Guid.Parse("00000000-0000-0000-0000-000000000001"));
        _mockPasskeyService
            .Setup(s => s.GetUserPasskeysAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([new WebAuthnDto
            {
                Id = Guid.NewGuid(),
                CredentialId = "private-credential-id",
                PublicKey = "private-public-key",
                SignatureCounter = 12,
                FriendlyName = "Laptop"
            }]);

        var result = await _authController.PasskeysAsync() as ObjectResult;

        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
        var passkeys = Assert.IsAssignableFrom<IEnumerable<PasskeyCredentialApiResponse>>(result.Value);
        Assert.Equal("Laptop", Assert.Single(passkeys).FriendlyName);
    }
}
