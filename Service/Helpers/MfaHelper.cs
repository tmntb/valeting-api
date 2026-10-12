using Common.Messages;
using OtpNet;
using Service.Models.User;

namespace Service.Helpers;

internal static class MfaHelper
{
    internal static void VerifyMfaCode(UserDto userDto, string mfaCode)
    {
        var secretBytes = Base32Encoding.ToBytes(userDto.MfaSecret);
        var totp = new Totp(secretBytes);
        var isValid = totp.VerifyTotp(mfaCode, out _, new VerificationWindow(previous: 1, future: 1));
        if (!isValid)
        {
            throw new InvalidOperationException(Messages.InvalidMfaCode);
        }
    }
}
