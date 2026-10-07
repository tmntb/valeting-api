namespace Api.Models.Auth.Payload;

public class MfaSetupApiResponse
{
    /// <summary>
    /// The URI for the QR code to set up MFA in an authenticator app.
    /// </summary>
    public string MfaQrCodeUri { get; set; }
}
