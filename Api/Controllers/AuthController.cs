using Api.Controllers.BaseController;
using Api.Models.Auth.Payload;
using Asp.Versioning;
using Common.Enums;
using Common.Messages;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using Service.Models.User;

namespace Api.Controllers;

[ApiVersion("1")]
public class AuthController(IAuthService authService) : AuthBaseController
{
    /// <inheritdoc />
    public override async Task<IActionResult> MfaEnableAsync([FromBody] MfaEnableApiRequest mfaEnableApiRequest)
    {
        ArgumentNullException.ThrowIfNull(mfaEnableApiRequest, Messages.InvalidRequestBody);

        await authService.MfaEnableAsync(new()
        {
            Email = mfaEnableApiRequest.Email,
            MfaCode = mfaEnableApiRequest.MfaCode
        });

        return NoContent();
    }

    /// <inheritdoc />
    public override Task<IActionResult> MfaRegenerateRecoveryCodesAsync()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public override async Task<IActionResult> MfaSetupAsync([FromBody] MfaSetupApiRequest mfaSetupApiRequest)
    {
        ArgumentNullException.ThrowIfNull(mfaSetupApiRequest, Messages.InvalidRequestBody);

        var mfaSetupDtoResponse = await authService.MfaSetupAsync(mfaSetupApiRequest.Email);

        return Ok(new MfaSetupApiResponse
        {
            MfaQrCodeUri = mfaSetupDtoResponse.MfaQrCodeUri,
            RecoveryCodes = mfaSetupDtoResponse.RecoveryCodes
        });
    }

    /// <inheritdoc />
    public override async Task<IActionResult> RegisterAsync([FromBody] RegisterApiRequest registerApiRequest)
    {
        ArgumentNullException.ThrowIfNull(registerApiRequest, Messages.InvalidRequestBody);

        var registerDtoRequest = new UserDto
        {
            Email = registerApiRequest.Email.Trim().ToLowerInvariant(),
            Password = registerApiRequest.Password,
            FirstName = registerApiRequest.FirstName,
            LastName = registerApiRequest.LastName,
            DateOfBirth = registerApiRequest.DateOfBirth,
            ContactNumber = registerApiRequest.ContactNumber,
            Role = new()
            {
                Code = RoleEnum.USER
            }
        };
        await authService.RegisterAsync(registerDtoRequest);

        return Created();
    }
}
