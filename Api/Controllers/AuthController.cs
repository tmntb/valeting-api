using System.Security.Claims;
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
    public override async Task<IActionResult> ForgotPasswordAsync([FromBody] ForgotPasswordApiRequest forgotPasswordApiRequest)
    {
        ArgumentNullException.ThrowIfNull(forgotPasswordApiRequest, Messages.InvalidRequestBody);

        await authService.ForgotPasswordAsync(new()
        {
            Email = forgotPasswordApiRequest.Email,
            MfaCode = forgotPasswordApiRequest.MfaCode,
            RecoveryCode = forgotPasswordApiRequest.RecoveryCode,
            NewPassword = forgotPasswordApiRequest.NewPassword
        });

        return Ok();
    }

    /// <inheritdoc />
    public override async Task<IActionResult> LoginAsync([FromBody] LoginApiRequest loginApiRequest)
    {
        ArgumentNullException.ThrowIfNull(loginApiRequest, Messages.InvalidRequestBody);

        var userDto = new UserDto
        {
            Email = loginApiRequest.Email,
            Password = loginApiRequest.Password
        };

        await authService.ValidateLoginAsync(userDto);

        var generateTokenJWTDtoResponse = await authService.GenerateTokenJWTAsync(loginApiRequest.Email);

        var validateLoginApiResponse = new LoginApiResponse
        {
            Token = generateTokenJWTDtoResponse.Token,
            TokenType = generateTokenJWTDtoResponse.TokenType,
            ExpiryDate = generateTokenJWTDtoResponse.ExpiryDate
        };
        return Ok(validateLoginApiResponse);
    }

    /// <inheritdoc />
    public override async Task<IActionResult> MfaEnableAsync([FromBody] MfaCodeApiRequest mfaCodeApiRequest)
    {
        ArgumentNullException.ThrowIfNull(mfaCodeApiRequest, Messages.InvalidRequestBody);

        await authService.MfaEnableAsync(new()
        {
            UserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value),
            MfaCode = mfaCodeApiRequest.MfaCode
        });

        return NoContent();
    }

    /// <inheritdoc />
    public override async Task<IActionResult> MfaRegenerateRecoveryCodesAsync([FromBody] MfaCodeApiRequest mfaCodeApiRequest)
    {
        ArgumentNullException.ThrowIfNull(mfaCodeApiRequest, Messages.InvalidRequestBody);

        var mfaRegenerateRecoveryCodes = await authService.MfaRegenerateRecoveryCodesAsync(new()
        {
            UserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value),
            MfaCode = mfaCodeApiRequest.MfaCode
        });

        return Ok(new MfaRegenerateRecoveryCodesApiResponse
        {
            RecoveryCodes = mfaRegenerateRecoveryCodes
        });
    }

    /// <inheritdoc />
    public override async Task<IActionResult> MfaSetupAsync()
    {
        var mfaSetupDtoResponse = await authService.MfaSetupAsync(Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value));

        return Ok(new MfaSetupApiResponse
        {
            MfaQrCodeUri = mfaSetupDtoResponse.MfaQrCodeUri,
            RecoveryCodes = mfaSetupDtoResponse.RecoveryCodes
        });
    }

    /// <inheritdoc />
    public override async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenApiRequest refreshTokenApiRequest)
    {
        ArgumentNullException.ThrowIfNull(refreshTokenApiRequest, Messages.InvalidRequestBody);
        ArgumentException.ThrowIfNullOrEmpty(refreshTokenApiRequest.Token, Messages.InvalidRequestBody);

        var email = authService.ValidateToken(refreshTokenApiRequest.Token); // TODO: Remove when middleware is implemented
        var generateTokenJwtDtoResponse = await authService.GenerateTokenJWTAsync(email);

        var refreshTokenApiResponse = new RefreshTokenApiResponse
        {
            Token = generateTokenJwtDtoResponse.Token,
            TokenType = generateTokenJwtDtoResponse.TokenType,
            ExpiryDate = generateTokenJwtDtoResponse.ExpiryDate
        };
        return Ok(refreshTokenApiResponse);
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
