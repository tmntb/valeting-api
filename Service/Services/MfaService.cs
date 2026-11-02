using Common.Messages;
using OtpNet;
using Service.Helpers;
using Service.Interfaces;
using Service.Models.Mfa.Payload;
using Service.Validators.Auth;
using Service.Validators.Utils;

namespace Service.Services;

public class MfaService(IUserRepository userRepository) : IMfaService
{
    /// <inheritdoc />
    public async Task<MfaSetupDtoResponse> MfaSetupAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var userDto = await userRepository.GetByIdAsync(userId, cancellationToken) ?? throw new KeyNotFoundException(Messages.NotFound);
        if (userDto.MfaEnabled)
        {
            throw new InvalidOperationException(Messages.MfaActivated);
        }

        if (userDto is { MfaSecret: not null, MfaEnabled: false })
        {
            return new()
            {
                MfaQrCodeUri = $"otpauth://totp/Valeting:{Uri.EscapeDataString(userDto.Email)}?secret={userDto.MfaSecret}&issuer=Valeting"
            };
        }

        var key = KeyGeneration.GenerateRandomKey(20);
        var mfaSecret = Base32Encoding.ToString(key);

        userDto.MfaSecret = mfaSecret;
        await userRepository.UpdateMfaSecretAsync(userDto, cancellationToken);

        return new()
        {
            MfaQrCodeUri = $"otpauth://totp/Valeting:{Uri.EscapeDataString(userDto.Email)}?secret={mfaSecret}&issuer=Valeting"
        };
    }

    /// <inheritdoc />
    public async Task MfaVerifyAsync(MfaCodeDtoRequest mfaCodeDtoRequest, CancellationToken cancellationToken = default)
    {
        mfaCodeDtoRequest.ValidateRequest(new MfaCodeVerifyValidator());

        var userDto = await userRepository.GetByIdAsync(mfaCodeDtoRequest.UserId, cancellationToken) ?? throw new KeyNotFoundException(Messages.NotFound);

        MfaHelper.VerifyMfaCode(userDto, mfaCodeDtoRequest.MfaCode);

        if (!userDto.MfaEnabled)
        {
            userDto.MfaEnabled = true;
            await userRepository.UpdateMfaEnableAsync(userDto, cancellationToken);
        }
    }
}
