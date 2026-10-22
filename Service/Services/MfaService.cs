using Common.Messages;
using OtpNet;
using Service.Helpers;
using Service.Interfaces;
using Service.Models.Auth.Payload;
using Service.Models.Mfa.Payload;
using Service.Validators.Auth;
using Service.Validators.Utils;

namespace Service.Services;

public class MfaService(IUserRepository userRepository) : IMfaService
{
    /// <inheritdoc />
    public async Task<MfaSetupDtoResponse> MfaSetupAsync(Guid userId)
    {
        var userDto = await userRepository.GetByIdAsync(userId) ?? throw new KeyNotFoundException(Messages.NotFound);
        if (userDto.MfaEnabled)
        {
            throw new InvalidOperationException(Messages.MfaActivated);
        }

        if (userDto.MfaSecret != null && !userDto.MfaEnabled)
        {
            return new()
            {
                MfaQrCodeUri = $"otpauth://totp/Valeting:{Uri.EscapeDataString(userDto.Email)}?secret={userDto.MfaSecret}&issuer=Valeting"
            };
        }

        var key = KeyGeneration.GenerateRandomKey(20);
        var mfaSecret = Base32Encoding.ToString(key);

        userDto.MfaSecret = mfaSecret;
        await userRepository.UpdateMfaSecretAsync(userDto);

        return new()
        {
            MfaQrCodeUri = $"otpauth://totp/Valeting:{Uri.EscapeDataString(userDto.Email)}?secret={mfaSecret}&issuer=Valeting"
        };
    }

    /// <inheritdoc />
    public async Task MfaVerifyAsync(MfaCodeDtoRequest mfaCodeDtoRequest)
    {
        mfaCodeDtoRequest.ValidateRequest(new MfaCodeVerifyValidator());

        var userDto = await userRepository.GetByIdAsync(mfaCodeDtoRequest.UserId) ?? throw new KeyNotFoundException(Messages.NotFound);

        MfaHelper.VerifyMfaCode(userDto, mfaCodeDtoRequest.MfaCode);

        if (!userDto.MfaEnabled)
        {
            userDto.MfaEnabled = true;
            await userRepository.UpdateMfaEnableAsync(userDto);
        }
    }
}
