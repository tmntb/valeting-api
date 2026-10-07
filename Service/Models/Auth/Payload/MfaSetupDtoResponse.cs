namespace Service.Models.Auth.Payload;

public class MfaSetupDtoResponse
{
    /// <summary>
    /// The URI for the QR code to set up MFA in an authenticator app.
    /// </summary>
    public string MfaQrCodeUri { get; set; }
}
