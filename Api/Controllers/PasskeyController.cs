using Api.Controllers.BaseController;
using Api.Extensions;
using Api.Models.Passkey.Payload;
using Asp.Versioning;
using Common.Messages;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace Api.Controllers;

[ApiVersion("1")]
public class PasskeyController(IPasskeyService passkeyService) : PasskeyBaseController
{
    /// <inheritdoc />
    public override async Task<IActionResult> PasskeyRegisterOptionsAsync()
    {
        var userId = User.GetCurrentUserId();
        var options = await passkeyService.CreateRegistrationOptionsAsync(userId);

        return Ok(new PasskeyRegisterOptionsApiResponse
        {
            CeremonyId = options.CeremonyId,
            PublicKey = options.PublicKey
        });
    }

    /// <inheritdoc />
    public override async Task<IActionResult> PasskeyRegisterVerifyAsync([FromBody] PasskeyRegisterVerifyApiRequest passkeyRegisterVerifyApiRequest)
    {
        ArgumentNullException.ThrowIfNull(passkeyRegisterVerifyApiRequest, Messages.InvalidRequestBody);

        var userId = User.GetCurrentUserId();
        await passkeyService.RegisterCredentialAsync(new()
        {
            UserId = userId,
            CeremonyId = passkeyRegisterVerifyApiRequest.CeremonyId,
            Credential = passkeyRegisterVerifyApiRequest.Credential,
            Name = passkeyRegisterVerifyApiRequest.Name
        });

        return NoContent();
    }
}
