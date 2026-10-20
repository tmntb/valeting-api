using System.Text;
using System.Text.Json;
using Common.Messages;
using Fido2NetLib;
using Fido2NetLib.Objects;
using Service.Interfaces;
using Service.Models.Auth;
using Service.Models.Passkey;
using Service.Models.User;
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
        webAuthnChallengeStore.Store(new PasskeyChallengeState
        {
            CeremonyId = ceremonyId,
            Ceremony = PasskeyCeremony.Authentication,
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

    public async Task<PasskeyAuthenticationResult> VerifyAuthenticationAsync(PasskeyAuthenticationVerifyRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.CeremonyId == Guid.Empty || request.Credential == null)
        {
            throw new ArgumentException("The passkey ceremony and credential response are required.", nameof(request));
        }

        if (!webAuthnChallengeStore.TryConsume(request.CeremonyId, PasskeyCeremony.Authentication, out var state))
        {
            throw new UnauthorizedAccessException("The passkey authentication ceremony is invalid, expired, or already used.");
        }

        var credentialId = request.Credential.Id;
        var passkey = await webAuthnRepository.GetByCredentialIdAsync(credentialId, cancellationToken)
            ?? throw new UnauthorizedAccessException("The passkey assertion could not be verified.");
        var user = await userRepository.GetByIdAsync(passkey.UserId);
        if (user is not { IsActive: true }
            || state!.UserId.HasValue && state.UserId.Value != user.Id
            || state.Email != null && !string.Equals(state.Email, user.Email, StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedAccessException("The passkey assertion could not be verified.");
        }

        var originalOptions = AssertionOptions.FromJson(state.OptionsJson);
        var storedCredentialId = DecodeBase64Url(passkey.CredentialId);
        VerifyAssertionResult result;
        try
        {
            result = await fido2.MakeAssertionAsync(new MakeAssertionParams
            {
                AssertionResponse = request.Credential,
                OriginalOptions = originalOptions,
                StoredPublicKey = DecodeBase64Url(passkey.PublicKey),
                StoredSignatureCounter = checked((uint)(passkey.SignatureCounter ?? 0)),
                IsUserHandleOwnerOfCredentialIdCallback = (args, _) =>
                {
                    var credentialMatches = args.CredentialId.SequenceEqual(storedCredentialId);
                    var handleMatches = args.UserHandle == null
                        || Guid.TryParseExact(Encoding.UTF8.GetString(args.UserHandle), "N", out var userId)
                        && userId == passkey.UserId;
                    return Task.FromResult(credentialMatches && handleMatches);
                }
            }, cancellationToken);
        }
        catch (Fido2VerificationException)
        {
            throw new UnauthorizedAccessException("The passkey assertion could not be verified.");
        }

        passkey.SignatureCounter = result.SignCount;
        passkey.LastUsedAt = DateTime.UtcNow;
        await webAuthnRepository.UpdateAsync(passkey, cancellationToken);
        await userRepository.UpdateLastLoginAsync(user.Id);

        return new PasskeyAuthenticationResult
        {
            Success = true,
            UserId = user.Id,
            Email = user.Email
        };
    }

    /// <inheritdoc />
    public async Task<WebAuthnRegistrationOptionsDtoResponse> CreateRegistrationOptionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var userDto = await userRepository.GetByIdAsync(userId);
        if (userDto == null || !userDto.IsActive)
        {
            throw new KeyNotFoundException(Messages.NotFound);
        }

        var passkeyDtoList = await webAuthnRepository.GetByUserIdAsync(userId, cancellationToken);
        var fidoUser = new Fido2User
        {
            Id = Encoding.UTF8.GetBytes(userDto.Id.ToString("N")),
            DisplayName = $"{userDto.FirstName} {userDto.LastName}".Trim(),
            Name = userDto.Email
        };

        var options = fido2.RequestNewCredential(new RequestNewCredentialParams
        {
            User = fidoUser,
            ExcludeCredentials = passkeyDtoList
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
        webAuthnChallengeStore.Store(new PasskeyChallengeState
        {
            CeremonyId = ceremonyId,
            Ceremony = PasskeyCeremony.Registration,
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
    public async Task VerifyRegisterCredentialAsync(WebAuthnRegistrationVerifyDtoRequest webAuthnRegistrationVerifyDtoRequest, CancellationToken cancellationToken = default)
    {
        webAuthnRegistrationVerifyDtoRequest.ValidateRequest(new VerifyRegisterCredentialsValidator());

        if (!webAuthnChallengeStore.TryConsume(webAuthnRegistrationVerifyDtoRequest.CeremonyId, PasskeyCeremony.Registration, out var state)
            || state!.UserId != webAuthnRegistrationVerifyDtoRequest.UserId)
        {
            throw new UnauthorizedAccessException("The passkey registration ceremony is invalid, expired, or already used.");
        }

        var originalOptions = CredentialCreateOptions.FromJson(state.OptionsJson);
        if (await webAuthnRepository.GetByCredentialIdAsync(webAuthnRegistrationVerifyDtoRequest.Credential.Id, cancellationToken) != null)
        {
            throw new InvalidOperationException("This passkey is already registered.");
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
                        await webAuthnRepository.GetByCredentialIdAsync(EncodeBase64Url(args.CredentialId), token) == null
                }, 
                cancellationToken
            );
        }
        catch (Fido2VerificationException)
        {
            throw new ArgumentException("The passkey registration response could not be verified.", nameof(webAuthnRegistrationVerifyDtoRequest));
        }

        var passkeyDto = new WebAuthnDto
        {
            Id = Guid.NewGuid(),
            UserId = webAuthnRegistrationVerifyDtoRequest.UserId,
            CredentialId = EncodeBase64Url(credential.Id),
            PublicKey = EncodeBase64Url(credential.PublicKey),
            FriendlyName = string.IsNullOrWhiteSpace(webAuthnRegistrationVerifyDtoRequest.Name) ? "Passkey" : webAuthnRegistrationVerifyDtoRequest.Name.Trim(),
            Aaguid = credential.AaGuid.ToString(),
            Transports = JsonSerializer.Serialize(credential.Transports),
            CreatedAt = DateTime.UtcNow,
            LastUsedAt = null,
            SignatureCounter = credential.SignCount
        };

        await webAuthnRepository.CreateAsync(passkeyDto, cancellationToken);
    }

    public async Task<IEnumerable<WebAuthnDto>> GetUserPasskeysAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await webAuthnRepository.GetByUserIdAsync(userId, cancellationToken);
    }

    public async Task RenamePasskeyAsync(Guid userId, Guid passkeyId, string friendlyName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(friendlyName) || friendlyName.Trim().Length > 200)
        {
            throw new ArgumentException("Friendly name is required and must not exceed 200 characters.", nameof(friendlyName));
        }

        var passkey = (await webAuthnRepository.GetByUserIdAsync(userId, cancellationToken)).SingleOrDefault(x => x.Id == passkeyId)
            ?? throw new KeyNotFoundException("Passkey not found for the current user.");

        passkey.FriendlyName = friendlyName.Trim();
        await webAuthnRepository.UpdateAsync(passkey, cancellationToken);
    }

    public async Task RemovePasskeyAsync(Guid userId, Guid passkeyId, CancellationToken cancellationToken = default)
    {
        var passkey = (await webAuthnRepository.GetByUserIdAsync(userId, cancellationToken)).SingleOrDefault(x => x.Id == passkeyId)
            ?? throw new KeyNotFoundException("Passkey not found for the current user.");

        await webAuthnRepository.DeleteAsync(passkey.Id, userId, cancellationToken);
    }

    private static string EncodeBase64Url(byte[] value)
    {
        return Convert.ToBase64String(value)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    private static byte[] DecodeBase64Url(string value)
    {
        var normalized = value.Replace('-', '+').Replace('_', '/');
        normalized = normalized.PadRight(normalized.Length + (4 - normalized.Length % 4) % 4, '=');
        return Convert.FromBase64String(normalized);
    }
}
