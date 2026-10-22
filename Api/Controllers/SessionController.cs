using System.Security.Claims;
using Api.Controllers.BaseController;
using Api.Extensions;
using Api.Models.Auth.Payload;
using Api.Models.WebAuthn.Payload;
using Asp.Versioning;
using Common.Messages;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using Service.Models.User;

namespace Api.Controllers;

[ApiVersion("1")]
public class SessionController(IMfaService mfaService, IWebAuthnService webAuthnService) : SessionBaseController
{
    /// <inheritdoc />
    public override async Task<IActionResult> MfaSetupAsync()
    {
        var mfaSetupDtoResponse = await mfaService.MfaSetupAsync(Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value));

        return Ok(new MfaSetupApiResponse
        {
            MfaQrCodeUri = mfaSetupDtoResponse.MfaQrCodeUri
        });
    }

    /// <inheritdoc />
    public override async Task<IActionResult> MfaVerifyAsync([FromBody] MfaCodeApiRequest mfaCodeApiRequest)
    {
        ArgumentNullException.ThrowIfNull(mfaCodeApiRequest, Messages.InvalidRequestBody);

        await mfaService.MfaVerifyAsync(new()
        {
            UserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value),
            MfaCode = mfaCodeApiRequest.MfaCode
        });

        return NoContent();
    }
    
    /// <inheritdoc />
    public override async Task<IActionResult> WebAuthnAuthenticateOptionsAsync([FromBody] WebAuthnAuthenticateOptionsApiRequest webAuthnAuthenticateOptionsApiRequest)
    {
        ArgumentNullException.ThrowIfNull(webAuthnAuthenticateOptionsApiRequest, Messages.InvalidRequestBody);

        var options = await webAuthnService.CreateAuthenticationOptionsAsync(new UserDto()
        {
            Email = webAuthnAuthenticateOptionsApiRequest.Email
        });
        
        return Ok(new WebAuthnAuthenticateOptionsApiResponse
        {
            CeremonyId = options.CeremonyId,
            PublicKey = options.PublicKey
        });
    }
    
    /// <inheritdoc />
    public override async Task<IActionResult> WebAuthnAuthenticateVerifyAsync([FromBody] WebAuthnAuthenticateVerifyApiRequest webAuthnAuthenticateVerifyApiRequest)
    {
        ArgumentNullException.ThrowIfNull(webAuthnAuthenticateVerifyApiRequest, Messages.InvalidRequestBody);

        await webAuthnService.VerifyAuthenticationAsync(new()
        {
            CeremonyId = webAuthnAuthenticateVerifyApiRequest.CeremonyId,
            Credential = webAuthnAuthenticateVerifyApiRequest.Credential
        });

        return NoContent();
    }
    
    /// <inheritdoc />
    public override async Task<IActionResult> WebAuthnRegisterOptionsAsync()
    {
        var userId = User.GetCurrentUserId();
        var options = await webAuthnService.CreateRegistrationOptionsAsync(userId);

        return Ok(new WebAuthnRegisterOptionsApiResponse
        {
            CeremonyId = options.CeremonyId,
            PublicKey = options.PublicKey
        });
    }

    /// <inheritdoc />
    public override async Task<IActionResult> WebAuthnRegisterVerifyAsync([FromBody] WebAuthnRegisterVerifyApiRequest webAuthnRegisterVerifyApiRequest)
    {
        ArgumentNullException.ThrowIfNull(webAuthnRegisterVerifyApiRequest, Messages.InvalidRequestBody);

        var userId = User.GetCurrentUserId();
        await webAuthnService.VerifyRegisterAsync(new()
        {
            UserId = userId,
            CeremonyId = webAuthnRegisterVerifyApiRequest.CeremonyId,
            Credential = webAuthnRegisterVerifyApiRequest.Credential,
            Name = webAuthnRegisterVerifyApiRequest.Name
        });

        return NoContent();
    }
}
