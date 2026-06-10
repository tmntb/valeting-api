namespace Api.Models.Auth.Payload;

public class MfaSetupApiResponse
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
