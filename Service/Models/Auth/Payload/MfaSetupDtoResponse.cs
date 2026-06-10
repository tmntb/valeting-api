namespace Service.Models.Auth.Payload;

public class MfaSetupDtoResponse
{
    /// <summary>
    /// The URI for the QR code to set up MFA in an authenticator app.
    /// </summary>
    public string MfaQrCodeUri { get; set; }

    /// <summary>
    /// The list of recovery codes for MFA account recovery.
    /// </summary>
    public List<string> RecoveryCodes { get; set; }
}
