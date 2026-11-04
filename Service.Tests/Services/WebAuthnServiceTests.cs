using System.Text;
using System.Text.Json;
using Common.Messages;
using Fido2NetLib;
using Fido2NetLib.Objects;
using FluentValidation;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Moq;
using Service.Interfaces;
using Service.Models.User;
using Service.Models.WebAuthn;
using Service.Models.WebAuthn.Payload;
using Service.Services;

namespace Service.Tests.Services;

public class WebAuthnServiceTests
{
    private readonly UserDto _userDto;
    private readonly WebAuthnDto _webAuthnDto;

    private readonly Guid CeremonyId = Guid.Parse("00000000-0000-0000-0000-000000000009");

    private readonly Mock<IFido2> _mockFido2;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IWebAuthnRepository> _mockWebAuthnRepository;
    private readonly Mock<IWebAuthnChallengeStore> _mockWebAuthnChallengeStore;
    private readonly WebAuthnService _service;

    public WebAuthnServiceTests()
    {
        _userDto = DataFactory.CreateUserDto();
        _webAuthnDto = DataFactory.CreateWebAuthnDto();

        _mockFido2 = new Mock<IFido2>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockWebAuthnRepository = new Mock<IWebAuthnRepository>();
        _mockWebAuthnChallengeStore = new Mock<IWebAuthnChallengeStore>();

        _service = new WebAuthnService(_mockFido2.Object, _mockUserRepository.Object, _mockWebAuthnRepository.Object, _mockWebAuthnChallengeStore.Object);
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    public async Task CreateAuthenticationOptionsAsync_ThrowsForInvalidEmail(string email)
    {
        // Arrange
        _userDto.Email = email;

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _service.CreateAuthenticationOptionsAsync(_userDto));
    }

    [Fact]
    public async Task CreateAuthenticationOptionsAsync_ForActiveUser_UsesExistingCredentialsAndStoresChallenge()
    {
        // Arrange
        _mockUserRepository
            .Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(_userDto);

        _mockWebAuthnRepository
            .Setup(r => r.GetByUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([_webAuthnDto]);

        GetAssertionOptionsParams? capturedParams = null;
        _mockFido2
            .Setup(fido2 => fido2.GetAssertionOptions(It.IsAny<GetAssertionOptionsParams>()))
            .Callback<GetAssertionOptionsParams>(parameters => capturedParams = parameters)
            .Returns(CreateAssertionOptions());

        // Act
        var response = await _service.CreateAuthenticationOptionsAsync(_userDto);

        // Assert
        Assert.NotEqual(Guid.Empty, response.CeremonyId);
        Assert.Equal(JsonValueKind.Object, response.PublicKey.ValueKind);
        Assert.NotNull(capturedParams);
        Assert.Equal(UserVerificationRequirement.Required, capturedParams.UserVerification);
    }

    [Fact]
    public async Task CreateAuthenticationOptionsAsync_ForInactiveUser_DoesNotAllowCredentialsOrBindUserId()
    {
        // Arrange
        _userDto.IsActive = false;
        _mockUserRepository
            .Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(_userDto);

        GetAssertionOptionsParams? capturedParams = null;
        _mockFido2
            .Setup(fido2 => fido2.GetAssertionOptions(It.IsAny<GetAssertionOptionsParams>()))
            .Callback<GetAssertionOptionsParams>(parameters => capturedParams = parameters)
            .Returns(CreateAssertionOptions());

        // Act
        var response = await _service.CreateAuthenticationOptionsAsync(_userDto);

        // Assert
        Assert.NotNull(capturedParams);
        Assert.Empty(capturedParams.AllowedCredentials);
    }

    [Fact]
    public async Task VerifyAuthenticationAsync_ThrowsWhenChallengeCannotBeConsumed()
    {
        // Act & Assert
        var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _service.VerifyAuthenticationAsync(new WebAuthnAuthenticationVerifyRequest
            {
                CeremonyId = CeremonyId,
                Credential = new AuthenticatorAssertionRawResponse { Id = _webAuthnDto.CredentialId }
            }));

        Assert.Equal(Messages.InvalidWebAuthnCeremony, ex.Message);
    }

    [Fact]
    public async Task VerifyAuthenticationAsync_ThrowsWhenCredentialIsUnknown()
    {
        // Arrange
        WebAuthnChallengeState? state = null;
        _mockWebAuthnChallengeStore
            .Setup(x => x.TryConsume(It.IsAny<Guid>(), It.IsAny<WebAuthnCeremony>(), out state))
            .Returns(true);

        _mockWebAuthnRepository
            .Setup(r => r.GetByCredentialIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((WebAuthnDto?)null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _service.VerifyAuthenticationAsync(new WebAuthnAuthenticationVerifyRequest
            {
                CeremonyId = CeremonyId,
                Credential = new AuthenticatorAssertionRawResponse { Id = _webAuthnDto.CredentialId }
            }));

        Assert.Equal(Messages.WebAuthnAssertionCannotBeVerified, ex.Message);
    }

    [Fact]
    public async Task VerifyAuthenticationAsync_RejectsInactiveOrMismatchedUser()
    {
        // Arrange
        var state = new WebAuthnChallengeState
        {
            UserId = _userDto.Id,
            Email = "different@example.com"
        };
        _mockWebAuthnChallengeStore
            .Setup(x => x.TryConsume(It.IsAny<Guid>(), It.IsAny<WebAuthnCeremony>(), out state))
            .Returns(true);

        _mockWebAuthnRepository
            .Setup(r => r.GetByCredentialIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_webAuthnDto);

        _mockUserRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_userDto);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _service.VerifyAuthenticationAsync(new WebAuthnAuthenticationVerifyRequest
            {
                CeremonyId = CeremonyId,
                Credential = new AuthenticatorAssertionRawResponse { Id = _webAuthnDto.CredentialId }
            }));

        Assert.Equal(Messages.WebAuthnAssertionCannotBeVerified, ex.Message);
    }

    [Fact]
    public async Task VerifyAuthenticationAsync_MapsFidoVerificationFailureToUnauthorized()
    {
        // Arrange
        var state = new WebAuthnChallengeState
        {
            UserId = _userDto.Id,
            Email = _userDto.Email,
            OptionsJson = CreateCredentialOptions().ToJson(),
        };
        _mockWebAuthnChallengeStore
            .Setup(x => x.TryConsume(It.IsAny<Guid>(), It.IsAny<WebAuthnCeremony>(), out state))
            .Returns(true);

        _mockWebAuthnRepository
            .Setup(r => r.GetByCredentialIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_webAuthnDto);

        _mockUserRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_userDto);

        _mockFido2.Setup(fido2 => fido2.MakeAssertionAsync(It.IsAny<MakeAssertionParams>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Fido2VerificationException("invalid assertion"));

        // Act & Assert
        var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _service.VerifyAuthenticationAsync(new WebAuthnAuthenticationVerifyRequest
            {
                CeremonyId = CeremonyId,
                Credential = new AuthenticatorAssertionRawResponse { Id = _webAuthnDto.CredentialId }
            }));

        Assert.Equal(Messages.WebAuthnAssertionCannotBeVerified, ex.Message);
        _mockWebAuthnRepository.Verify(r => r.UpdateSignalCounter(It.IsAny<WebAuthnDto>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUserRepository.Verify(r => r.UpdateLastLoginAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task VerifyAuthenticationAsync_UpdatesCounterLastLoginAndChecksUserHandleOwnership()
    {
        // Arrange
        var state = new WebAuthnChallengeState
        {
            UserId = _userDto.Id,
            Email = _userDto.Email,
            OptionsJson = CreateCredentialOptions().ToJson(),
        };
        _mockWebAuthnChallengeStore
            .Setup(x => x.TryConsume(It.IsAny<Guid>(), It.IsAny<WebAuthnCeremony>(), out state))
            .Returns(true);

        _mockWebAuthnRepository
            .Setup(r => r.GetByCredentialIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_webAuthnDto);

        _mockUserRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_userDto);

        MakeAssertionParams? capturedParams = null;
        _mockFido2.Setup(fido2 => fido2.MakeAssertionAsync(It.IsAny<MakeAssertionParams>(), It.IsAny<CancellationToken>()))
            .Callback<MakeAssertionParams, CancellationToken>((parameters, _) => capturedParams = parameters)
            .ReturnsAsync(new VerifyAssertionResult { SignCount = 9 });

        // Act
        await _service.VerifyAuthenticationAsync(new WebAuthnAuthenticationVerifyRequest
        {
            CeremonyId = CeremonyId,
            Credential = new AuthenticatorAssertionRawResponse { Id = _webAuthnDto.CredentialId }
        });

        // Assert
        Assert.NotNull(capturedParams);
        Assert.Equal(_webAuthnDto.PublicKey, Convert.ToBase64String(capturedParams.StoredPublicKey));
        Assert.Equal((uint)3, capturedParams.StoredSignatureCounter);
        Assert.NotNull(capturedParams.IsUserHandleOwnerOfCredentialIdCallback);

        _mockWebAuthnRepository.Verify(r => r.UpdateSignalCounter(It.IsAny<WebAuthnDto>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUserRepository.Verify(r => r.UpdateLastLoginAsync(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task CreateRegistrationOptionsAsync_ThrowsWhenUserDoesNotExist()
    {
        // Arrange
        _mockUserRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((UserDto?)null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.CreateRegistrationOptionsAsync(_userDto.Id));
        Assert.Equal(Messages.NotFound, ex.Message);
    }

    [Fact]
    public async Task CreateRegistrationOptionsAsync_ThrowsWhenUserIsInactive()
    {
        // Arrange
        _userDto.IsActive = false;
        _mockUserRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_userDto);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.CreateRegistrationOptionsAsync(_userDto.Id));
        Assert.Equal(Messages.NotFound, ex.Message);
    }

    [Fact]
    public async Task CreateRegistrationOptionsAsync_UsesExistingCredentialExclusionsAndStoresChallenge()
    {
        // Arrange
        _mockUserRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_userDto);

        _mockWebAuthnRepository
            .Setup(r => r.GetByUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([_webAuthnDto]);

        RequestNewCredentialParams? capturedParams = null;
        _mockFido2
            .Setup(fido2 => fido2.RequestNewCredential(It.IsAny<RequestNewCredentialParams>()))
            .Callback<RequestNewCredentialParams>(parameters => capturedParams = parameters)
            .Returns(CreateCredentialOptions());

        // Act
        var response = await _service.CreateRegistrationOptionsAsync(_userDto.Id);

        // Assert
        Assert.NotEqual(Guid.Empty, response.CeremonyId);
        Assert.Equal(JsonValueKind.Object, response.PublicKey.ValueKind);
        Assert.NotNull(capturedParams);
        Assert.Equal(_userDto.Id.ToString("N"), Encoding.UTF8.GetString(capturedParams.User.Id));
        Assert.Equal("John Doe", capturedParams.User.DisplayName);
        Assert.Equal(_userDto.Email, capturedParams.User.Name);

        _mockWebAuthnChallengeStore.Verify(x => x.Store(It.IsAny<WebAuthnChallengeState>()), Times.Once);
    }

    [Fact]
    public async Task VerifyRegisterAsync_ThrowsWhenChallengeCannotBeConsumed()
    {
        // Act & Assert
        var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _service.VerifyRegisterAsync(new WebAuthnRegistrationVerifyDtoRequest
            {
                UserId = _userDto.Id,
                Name = "Name",
                CeremonyId = CeremonyId,
                Credential = new AuthenticatorAttestationRawResponse { Id = _webAuthnDto.CredentialId }
            }));

        Assert.Equal(Messages.InvalidWebAuthnCeremony, ex.Message);
    }

    [Fact]
    public async Task VerifyRegisterAsync_RejectsAlreadyRegisteredCredential()
    {
        // Arrange
        var state = new WebAuthnChallengeState
        {
            UserId = _userDto.Id,
            Email = _userDto.Email,
            OptionsJson = CreateCredentialOptions().ToJson(),
        };
        _mockWebAuthnChallengeStore
            .Setup(x => x.TryConsume(It.IsAny<Guid>(), It.IsAny<WebAuthnCeremony>(), out state))
            .Returns(true);

        _mockWebAuthnRepository
            .Setup(r => r.GetByCredentialIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_webAuthnDto);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.VerifyRegisterAsync(new WebAuthnRegistrationVerifyDtoRequest
        {
            UserId = _userDto.Id,
            Name = "Name",
            CeremonyId = CeremonyId,
            Credential = new AuthenticatorAttestationRawResponse { Id = _webAuthnDto.CredentialId }
        }));

        Assert.Equal(Messages.WebAuthnAlreadyRegistered, ex.Message);
        _mockFido2.Verify(fido2 => fido2.MakeNewCredentialAsync(It.IsAny<MakeNewCredentialParams>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task VerifyRegisterAsync_MapsFidoVerificationFailureToArgumentException()
    {
        // Arrange
        var state = new WebAuthnChallengeState
        {
            UserId = _userDto.Id,
            Email = _userDto.Email,
            OptionsJson = CreateCredentialOptions().ToJson(),
        };
        _mockWebAuthnChallengeStore
            .Setup(x => x.TryConsume(It.IsAny<Guid>(), It.IsAny<WebAuthnCeremony>(), out state))
            .Returns(true);

        _mockWebAuthnRepository
            .Setup(r => r.GetByCredentialIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((WebAuthnDto?)null);

        _mockFido2.Setup(fido2 => fido2.MakeNewCredentialAsync(It.IsAny<MakeNewCredentialParams>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Fido2VerificationException("invalid attestation"));

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.VerifyRegisterAsync(new WebAuthnRegistrationVerifyDtoRequest
        {
            UserId = _userDto.Id,
            Name = "Name",
            CeremonyId = CeremonyId,
            Credential = new AuthenticatorAttestationRawResponse { Id = _webAuthnDto.CredentialId }
        }));

        Assert.Equal(Messages.WebAuthnRegistrationCannotBeVerified, ex.Message);
        _mockWebAuthnRepository.Verify(r => r.CreateAsync(It.IsAny<WebAuthnDto>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task VerifyRegisterAsync_PersistsEncodedCredentialAndTrimmedName()
    {
        // Arrange
        var state = new WebAuthnChallengeState
        {
            UserId = _userDto.Id,
            Email = _userDto.Email,
            OptionsJson = CreateCredentialOptions().ToJson(),
        };
        _mockWebAuthnChallengeStore
            .Setup(x => x.TryConsume(It.IsAny<Guid>(), It.IsAny<WebAuthnCeremony>(), out state))
            .Returns(true);

        _mockWebAuthnRepository
            .Setup(r => r.GetByCredentialIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((WebAuthnDto?)null);

        MakeNewCredentialParams? capturedParams = null;
        _mockFido2.Setup(fido2 => fido2.MakeNewCredentialAsync(It.IsAny<MakeNewCredentialParams>(), It.IsAny<CancellationToken>()))
            .Callback<MakeNewCredentialParams, CancellationToken>((parameters, _) => capturedParams = parameters)
            .ReturnsAsync(new RegisteredPublicKeyCredential
            {
                Id = [1, 2],
                PublicKey = [4, 5],
                AaGuid = Guid.Parse("00000000-0000-0000-0000-000000000099"),
                Transports = [AuthenticatorTransport.Internal],
                SignCount = 4
            });

        _mockWebAuthnRepository
            .Setup(r => r.CreateAsync(It.IsAny<WebAuthnDto>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable(Times.Once);

        // Act
        await _service.VerifyRegisterAsync(new WebAuthnRegistrationVerifyDtoRequest
        {
            UserId = _userDto.Id,
            Name = "Name",
            CeremonyId = CeremonyId,
            Credential = new AuthenticatorAttestationRawResponse { Id = _webAuthnDto.CredentialId }
        });

        // Assert
        Assert.NotNull(capturedParams);
        Assert.NotNull(capturedParams.IsCredentialIdUniqueToUserCallback);
        Assert.True(await capturedParams.IsCredentialIdUniqueToUserCallback(new IsCredentialIdUniqueToUserParams([8], new Fido2User()), CancellationToken.None));
        
        _mockWebAuthnRepository.Verify();
    }

    [Fact]
    public async Task GetUserWebAuthnAsync_DelegatesToRepository()
    {
        // Arrange
        _mockWebAuthnRepository
            .Setup(r => r.GetByUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([_webAuthnDto]);
    
        // Act
        var result = await _service.GetUserWebAuthnAsync(_userDto.Id);
    
        // Assert
        Assert.NotEmpty(result);
        Assert.Single(result);
    }
    
    [Fact]
    public async Task RemoveWebAuthnAsync_DelegatesToRepository()
    {
        // Act
        await _service.RemoveWebAuthnAsync(_webAuthnDto.Id);
    
        // Assert
        _mockWebAuthnRepository.Verify(r => r.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    private AssertionOptions CreateAssertionOptions() => new()
    {
        Challenge = [7, 8, 9],
        Timeout = 60_000,
        RpId = "example.com",
        AllowCredentials = []
    };

    private CredentialCreateOptions CreateCredentialOptions() => new()
    {
        Rp = new PublicKeyCredentialRpEntity("example.com", "Example"),
        User = new Fido2User
        {
            Id = Encoding.UTF8.GetBytes(_userDto.Id.ToString("N")),
            Name = "user@example.com",
            DisplayName = "John Doe"
        },
        Challenge = [7, 8, 9],
        PubKeyCredParams = [],
        Timeout = 60_000,
        Attestation = AttestationConveyancePreference.None,
        AuthenticatorSelection = new AuthenticatorSelection
        {
            ResidentKey = ResidentKeyRequirement.Required,
            UserVerification = UserVerificationRequirement.Required
        },
        ExcludeCredentials = []
    };
}