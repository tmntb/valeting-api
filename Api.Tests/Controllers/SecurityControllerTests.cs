using System.Net;
using System.Text.Json;
using Api.Controllers;
using Api.Models.Auth.Payload;
using Api.Models.WebAuthn.Payload;
using Common.Messages;
using Fido2NetLib;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Service.Interfaces;
using Service.Models.Mfa.Payload;
using Service.Models.User;
using Service.Models.WebAuthn;
using Service.Models.WebAuthn.Payload;

namespace Api.Tests.Controllers;

public class SecurityControllerTests
{
    private static readonly Guid _mockUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    private readonly ClaimsFixture _claimsFixture = new();
    private readonly Mock<IMfaService> _mockMfaService;
    private readonly Mock<IWebAuthnService> _mockWebAuthnService;

    private readonly SecurityController _securityController;

    public SecurityControllerTests()
    {
        _mockMfaService = new Mock<IMfaService>();
        _mockWebAuthnService = new Mock<IWebAuthnService>();

        _securityController = new SecurityController(_mockMfaService.Object, _mockWebAuthnService.Object);
    }

    [Fact]
    public async Task MfaSetupAsync_ReturnsQrCodeUri()
    {
        // Arrange
        _claimsFixture.SetupUserClaims(_securityController, _mockUserId);

        _mockMfaService
            .Setup(service => service.MfaSetupAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new MfaSetupDtoResponse { MfaQrCodeUri = "otpauth://test" })
            .Verifiable(Times.Once);

        // Act
        var result = await _securityController.MfaSetupAsync() as ObjectResult;

        // Assert
        Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
        Assert.Equal("otpauth://test", Assert.IsType<MfaSetupApiResponse>(result.Value).MfaQrCodeUri);
        _mockMfaService.Verify();
    }

    [Fact]
    public async Task MfaVerifyAsync_ThrowsWhenRequestIsNull()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _securityController.MfaVerifyAsync(null));
        Assert.Contains(Messages.InvalidRequestBody, exception.Message);
    }

    [Fact]
    public async Task MfaVerifyAsync_ForwardsAuthenticatedUserAndCode()
    {
        // Arrange
        _claimsFixture.SetupUserClaims(_securityController, _mockUserId);

        _mockMfaService
            .Setup(s => s.MfaVerifyAsync(It.IsAny<MfaCodeDtoRequest>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _securityController.MfaVerifyAsync
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
    public async Task WebAuthnAuthenticateOptionsAsync_ThrowsWhenRequestIsNull()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _securityController.WebAuthnAuthenticateOptionsAsync(null!));

        Assert.Contains(Messages.InvalidRequestBody, exception.Message);
    }

    [Fact]
    public async Task WebAuthnAuthenticateOptionsAsync_ForwardsEmailAndMapsResponse()
    {
        // Arrange
        var ceremonyId = Guid.Parse("00000000-0000-0000-0000-000000000002");
        var publicKey = JsonDocument.Parse("""{"challenge":"sample"}""").RootElement.Clone();

        _mockWebAuthnService
            .Setup(s => s.CreateAuthenticationOptionsAsync(It.IsAny<UserDto>()))
            .ReturnsAsync(new WebAuthnAuthenticationOptionsDtoResponse
            {
                CeremonyId = ceremonyId,
                PublicKey = publicKey
            });

        // Act
        var result = await _securityController.WebAuthnAuthenticateOptionsAsync
        (
            new WebAuthnAuthenticateOptionsApiRequest { Email = "test@example.com" }
        ) as ObjectResult;

        // Assert
        var responseApi = (WebAuthnAuthenticateOptionsApiResponse)result.Value;
        Assert.Equal(ceremonyId, responseApi.CeremonyId);
        Assert.Equal(publicKey, responseApi.PublicKey);
    }

    [Fact]
    public async Task WebAuthnAuthenticateVerifyAsync_ThrowsWhenRequestIsNull()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _securityController.WebAuthnAuthenticateVerifyAsync(null!));

        Assert.Contains(Messages.InvalidRequestBody, exception.Message);
    }

    [Fact]
    public async Task WebAuthnAuthenticateVerifyAsync_ForwardsCeremonyAndCredential()
    {
        // Arrange
        var ceremonyId = Guid.Parse("00000000-0000-0000-0000-000000000002");
        var credential = new AuthenticatorAssertionRawResponse { Id = "credential-id" };
        WebAuthnAuthenticationVerifyRequest? capturedRequest = null;

        _mockWebAuthnService
            .Setup(s => s.VerifyAuthenticationAsync(It.IsAny<WebAuthnAuthenticationVerifyRequest>()))
            .Callback<WebAuthnAuthenticationVerifyRequest, CancellationToken>((request, _) => capturedRequest = request)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _securityController.WebAuthnAuthenticateVerifyAsync(new WebAuthnAuthenticateVerifyApiRequest
        {
            CeremonyId = ceremonyId,
            Credential = credential!
        });

        // Assert
        Assert.IsType<NoContentResult>(result);

        Assert.NotNull(capturedRequest);
        Assert.Equal(ceremonyId, capturedRequest.CeremonyId);
        Assert.Same(credential, capturedRequest.Credential);
    }

    [Fact]
    public async Task WebAuthnRegisterOptionsAsync_UsesAuthenticatedUserAndMapsResponse()
    {
        // Arrange
        _claimsFixture.SetupUserClaims(_securityController, _mockUserId);

        var ceremonyId = Guid.Parse("00000000-0000-0000-0000-000000000002");
        var publicKey = JsonDocument.Parse("""{"rp":{"name":"example"}}""").RootElement.Clone();

        _mockWebAuthnService
            .Setup(s => s.CreateRegistrationOptionsAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new WebAuthnRegistrationOptionsDtoResponse
            {
                CeremonyId = ceremonyId,
                PublicKey = publicKey
            });

        // Act
        var result = await _securityController.WebAuthnRegisterOptionsAsync() as ObjectResult;

        // Assert
        var response = Assert.IsType<WebAuthnRegisterOptionsApiResponse>(result.Value);

        Assert.Equal(ceremonyId, response.CeremonyId);
        Assert.Equal(publicKey, response.PublicKey);
    }

    [Fact]
    public async Task WebAuthnRegisterVerifyAsync_ThrowsWhenRequestIsNull()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _securityController.WebAuthnRegisterVerifyAsync(null!));

        Assert.Contains(Messages.InvalidRequestBody, exception.Message);
    }

    [Fact]
    public async Task WebAuthnRegisterVerifyAsync_ForwardsAuthenticatedUserAndRequest()
    {
        // Arrange
        _claimsFixture.SetupUserClaims(_securityController, _mockUserId);

        var ceremonyId = Guid.Parse("00000000-0000-0000-0000-000000000002");
        var credential = new AuthenticatorAttestationRawResponse { Id = "credential-id" };

        WebAuthnRegistrationVerifyDtoRequest? capturedRequest = null;
        _mockWebAuthnService
            .Setup(s => s.VerifyRegisterAsync(It.IsAny<WebAuthnRegistrationVerifyDtoRequest>()))
            .Callback<WebAuthnRegistrationVerifyDtoRequest, CancellationToken>((request, _) => capturedRequest = request)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _securityController.WebAuthnRegisterVerifyAsync(new WebAuthnRegisterVerifyApiRequest
        {
            CeremonyId = ceremonyId,
            Credential = credential!,
            Name = "My passkey"
        });

        // Assert
        Assert.IsType<NoContentResult>(result);
        Assert.NotNull(capturedRequest);
        Assert.Equal(_mockUserId, capturedRequest.UserId);
        Assert.Equal(ceremonyId, capturedRequest.CeremonyId);
        Assert.Same(credential, capturedRequest.Credential);
        Assert.Equal("My passkey", capturedRequest.Name);
    }

    [Fact]
    public async Task WebAuthnsAsync_ReturnsSafeCredentialMetadata()
    {
        // Arrange
        _claimsFixture.SetupUserClaims(_securityController, _mockUserId);

        var webauthnId = Guid.Parse("00000000-0000-0000-0000-000000000003");
        var createdAt = DateTime.UtcNow;
        var lastUsedAt = createdAt.AddMinutes(1);

        _mockWebAuthnService
            .Setup(service => service.GetUserWebAuthnAsync(It.IsAny<Guid>()))
            .ReturnsAsync([
                new WebAuthnDto
                {
                    Id = webauthnId,
                    FriendlyName = "Laptop",
                    CreatedAt = createdAt,
                    LastUsedAt = lastUsedAt,
                    CredentialId = "not returned",
                    PublicKey = "not returned"
                }
            ]);

        // Act
        var result = await _securityController.WebAuthnsAsync();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var credentials = Assert.IsType<IEnumerable<WebAuthnCredentialApiResponse>>(okResult.Value, exactMatch: false);
        var credential = Assert.Single(credentials);

        Assert.Equal(webauthnId, credential.Id);
        Assert.Equal("Laptop", credential.FriendlyName);
        Assert.Equal(createdAt, credential.CreatedAt);
        Assert.Equal(lastUsedAt, credential.LastUsedAt);
    }

    [Fact]
    public async Task RemoveWebAuthnAsync_DelegatesAndReturnsNoContent()
    {
        // Arrange
        var webauthnId = Guid.Parse("00000000-0000-0000-0000-000000000003");
        _mockWebAuthnService
            .Setup(service => service.RemoveWebAuthnAsync(It.IsAny<Guid>()))
            .Returns(Task.CompletedTask)
            .Verifiable(Times.Once);

        // Act
        var result = await _securityController.RemoveWebAuthnAsync(webauthnId);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _mockWebAuthnService.Verify();
    }
}