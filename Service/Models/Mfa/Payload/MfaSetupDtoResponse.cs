namespace Service.Models.Mfa.Payload;

/// <summary>
/// Contains the authenticator enrollment URI returned during MFA setup.
/// </summary>
public class MfaSetupDtoResponse
{
    /// <summary>
    /// The URI for the QR code to set up MFA in an authenticator app.
    /// </summary>
    public string MfaQrCodeUri { get; set; }
}
