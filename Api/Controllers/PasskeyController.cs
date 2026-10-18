using Api.Controllers.BaseController;
using Api.Extensions;
using Api.Models.Passkey.Payload;
using Asp.Versioning;
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
}
