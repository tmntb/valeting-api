using Common.Messages;
using OtpNet;
using Service.Models.User;

namespace Service.Helpers;

internal static class MfaHelper
{
    /// <summary>
    /// Verifies a time-based one-time password using the MFA secret associated with the user.
    /// </summary>
    /// <param name="userDto">The user whose MFA secret is used to validate the code.</param>
    /// <param name="mfaCode">The MFA code to verify.</param>
    /// <exception cref="InvalidOperationException">Thrown when the MFA code is invalid.</exception>
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
