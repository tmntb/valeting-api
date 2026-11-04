using System.Text;
using System.Text.Json;
using Common.Messages;
using Fido2NetLib;
using Fido2NetLib.Objects;
using Service.Interfaces;
using Service.Models.User;
using Service.Models.WebAuthn;
using Service.Models.WebAuthn.Payload;
using Service.Validators.Utils;
using Service.Validators.WebAuthn;

namespace Service.Services;

public class WebAuthnService(IFido2 fido2, IUserRepository userRepository, IWebAuthnRepository webAuthnRepository, IWebAuthnChallengeStore webAuthnChallengeStore) : IWebAuthnService
{
    private static readonly TimeSpan ChallengeLifetime = TimeSpan.FromMinutes(5);

    /// <inheritdoc />
    public async Task<WebAuthnAuthenticationOptionsDtoResponse> CreateAuthenticationOptionsAsync(UserDto userDto, CancellationToken cancellationToken = default)
    {
        userDto.ValidateRequest(new CreateAuthenticationOptionsValidator());

        var userDtoToCheck = await userRepository.GetByEmailAsync(userDto.Email);

        var credentials = userDtoToCheck.IsActive
            ? (await webAuthnRepository.GetByUserIdAsync(userDtoToCheck.Id, cancellationToken))
            .Select(x => new PublicKeyCredentialDescriptor(DecodeBase64Url(x.CredentialId)))
            .ToList()
            : [];

        var options = fido2.GetAssertionOptions(new GetAssertionOptionsParams
        {
            AllowedCredentials = credentials,
            UserVerification = UserVerificationRequirement.Required
        });

        var ceremonyId = Guid.NewGuid();
        var optionsJson = options.ToJson();
        webAuthnChallengeStore.Store(new WebAuthnChallengeState
        {
            CeremonyId = ceremonyId,
            Ceremony = WebAuthnCeremony.Authentication,
            UserId = userDtoToCheck.IsActive ? userDtoToCheck.Id : null,
            Email = userDtoToCheck.Email,
            OptionsJson = optionsJson,
            ExpiresAt = DateTimeOffset.UtcNow.Add(ChallengeLifetime)
        });

        return new WebAuthnAuthenticationOptionsDtoResponse
        {
            CeremonyId = ceremonyId,
            PublicKey = JsonDocument.Parse(optionsJson).RootElement.Clone()
        };
    }

    /// <inheritdoc />
    public async Task VerifyAuthenticationAsync(WebAuthnAuthenticationVerifyRequest webAuthnAuthenticationVerifyRequest, CancellationToken cancellationToken = default)
    {
        webAuthnAuthenticationVerifyRequest.ValidateRequest(new VerifyAuthenticationValidator());

        if (!webAuthnChallengeStore.TryConsume(webAuthnAuthenticationVerifyRequest.CeremonyId, WebAuthnCeremony.Authentication, out var state))
        {
            throw new UnauthorizedAccessException(Messages.InvalidWebAuthnCeremony);
        }

        var webAuthnDto = await webAuthnRepository.GetByCredentialIdAsync(webAuthnAuthenticationVerifyRequest.Credential.Id, cancellationToken)
            ?? throw new UnauthorizedAccessException(Messages.WebAuthnAssertionCannotBeVerified);

        var userDto = await userRepository.GetByIdAsync(webAuthnDto.UserId);
        if (!userDto.IsActive
            || state!.UserId.HasValue && state.UserId.Value != userDto.Id
            || state.Email != null && !string.Equals(state.Email, userDto.Email, StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedAccessException(Messages.WebAuthnAssertionCannotBeVerified);
        }

        var originalOptions = AssertionOptions.FromJson(state.OptionsJson);
        var storedCredentialId = DecodeBase64Url(webAuthnDto.CredentialId);
        VerifyAssertionResult result;
        try
        {
            result = await fido2.MakeAssertionAsync(new MakeAssertionParams
            {
                AssertionResponse = webAuthnAuthenticationVerifyRequest.Credential,
                OriginalOptions = originalOptions,
                StoredPublicKey = DecodeBase64Url(webAuthnDto.PublicKey),
                StoredSignatureCounter = checked((uint)(webAuthnDto.SignatureCounter ?? 0)),
                IsUserHandleOwnerOfCredentialIdCallback = (args, _) =>
                {
                    var credentialMatches = args.CredentialId.SequenceEqual(storedCredentialId);
                    var handleMatches =
                        Guid.TryParseExact(Encoding.UTF8.GetString(args.UserHandle), "N", out var userId)
                        && userId == webAuthnDto.UserId;
                    return Task.FromResult(credentialMatches && handleMatches);
                }
            }, cancellationToken);
        }
        catch (Fido2VerificationException)
        {
            throw new UnauthorizedAccessException(Messages.WebAuthnAssertionCannotBeVerified);
        }

        webAuthnDto.SignatureCounter = result.SignCount;

        await webAuthnRepository.UpdateSignalCounter(webAuthnDto, cancellationToken);
        await userRepository.UpdateLastLoginAsync(userDto.Id);
    }

    /// <inheritdoc />
    public async Task<WebAuthnRegistrationOptionsDtoResponse> CreateRegistrationOptionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var userDto = await userRepository.GetByIdAsync(userId);
        if (userDto == null || !userDto.IsActive)
        {
            throw new KeyNotFoundException(Messages.NotFound);
        }

        var webAuthnDtos = await webAuthnRepository.GetByUserIdAsync(userId, cancellationToken);
        var fidoUser = new Fido2User
        {
            Id = Encoding.UTF8.GetBytes(userDto.Id.ToString("N")),
            DisplayName = $"{userDto.FirstName} {userDto.LastName}".Trim(),
            Name = userDto.Email
        };

        var options = fido2.RequestNewCredential(new RequestNewCredentialParams
        {
            User = fidoUser,
            ExcludeCredentials = webAuthnDtos
                .Select(x => new PublicKeyCredentialDescriptor(DecodeBase64Url(x.CredentialId)))
                .ToList(),
            AuthenticatorSelection = new AuthenticatorSelection
            {
                ResidentKey = ResidentKeyRequirement.Required,
                UserVerification = UserVerificationRequirement.Required
            },
            AttestationPreference = AttestationConveyancePreference.None
        });

        var ceremonyId = Guid.NewGuid();
        var optionsJson = options.ToJson();
        webAuthnChallengeStore.Store(new WebAuthnChallengeState
        {
            CeremonyId = ceremonyId,
            Ceremony = WebAuthnCeremony.Registration,
            UserId = userId,
            OptionsJson = optionsJson,
            ExpiresAt = DateTimeOffset.UtcNow.Add(ChallengeLifetime)
        });

        return new()
        {
            CeremonyId = ceremonyId,
            PublicKey = JsonDocument.Parse(optionsJson).RootElement.Clone()
        };
    }

    /// <inheritdoc />
    public async Task VerifyRegisterAsync(WebAuthnRegistrationVerifyDtoRequest webAuthnRegistrationVerifyDtoRequest, CancellationToken cancellationToken = default)
    {
        webAuthnRegistrationVerifyDtoRequest.ValidateRequest(new VerifyRegisterValidator());

        if (!webAuthnChallengeStore.TryConsume(webAuthnRegistrationVerifyDtoRequest.CeremonyId, WebAuthnCeremony.Registration, out var state)
            || state!.UserId != webAuthnRegistrationVerifyDtoRequest.UserId)
        {
            throw new UnauthorizedAccessException(Messages.InvalidWebAuthnCeremony);
        }

        var originalOptions = CredentialCreateOptions.FromJson(state.OptionsJson);

        if (await webAuthnRepository.GetByCredentialIdAsync(webAuthnRegistrationVerifyDtoRequest.Credential.Id, cancellationToken) != null)
        {
            throw new InvalidOperationException(Messages.WebAuthnAlreadyRegistered);
        }

        RegisteredPublicKeyCredential credential;
        try
        {
            credential = await fido2.MakeNewCredentialAsync(
                new MakeNewCredentialParams
                {
                    AttestationResponse = webAuthnRegistrationVerifyDtoRequest.Credential,
                    OriginalOptions = originalOptions,
                    IsCredentialIdUniqueToUserCallback = async (args, token) =>
                        await webAuthnRepository.GetByCredentialIdAsync(EncodeBase64Url(args.CredentialId), token) ==
                        null
                },
                cancellationToken
            );
        }
        catch (Fido2VerificationException)
        {
            throw new ArgumentException(Messages.WebAuthnRegistrationCannotBeVerified);
        }

        var webAuthnDto = new WebAuthnDto
        {
            Id = Guid.NewGuid(),
            UserId = webAuthnRegistrationVerifyDtoRequest.UserId,
            CredentialId = EncodeBase64Url(credential.Id),
            PublicKey = EncodeBase64Url(credential.PublicKey),
            FriendlyName = webAuthnRegistrationVerifyDtoRequest.Name.Trim(),
            Aaguid = credential.AaGuid.ToString(),
            Transports = JsonSerializer.Serialize(credential.Transports),
            CreatedAt = DateTime.UtcNow,
            LastUsedAt = null,
            SignatureCounter = credential.SignCount
        };

        await webAuthnRepository.CreateAsync(webAuthnDto, cancellationToken);
    }

    /// <inheritdoc /> 
    public async Task<IEnumerable<WebAuthnDto>> GetUserWebAuthnAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await webAuthnRepository.GetByUserIdAsync(userId, cancellationToken);
    }

    /// <inheritdoc /> 
    public async Task RemoveWebAuthnAsync(Guid webauthnId, CancellationToken cancellationToken = default) =>
        await webAuthnRepository.DeleteAsync(webauthnId, cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private static string EncodeBase64Url(byte[] value)
    {
        return Convert.ToBase64String(value)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private static byte[] DecodeBase64Url(string value)
    {
        var normalized = value.Replace('-', '+').Replace('_', '/');
        normalized = normalized.PadRight(normalized.Length + (4 - normalized.Length % 4) % 4, '=');
        return Convert.FromBase64String(normalized);
    }
}