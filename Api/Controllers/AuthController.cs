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
public class AuthController(IAuthService authService, IRecoveryCodeService recoveryCodeService, ITokenService tokenService) : AuthBaseController
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

        var generateTokenJwtDtoResponse = await tokenService.GenerateTokenJwtAsync(loginApiRequest.Email);

        var validateLoginApiResponse = new LoginApiResponse
        {
            Token = generateTokenJwtDtoResponse.Token,
            TokenType = generateTokenJwtDtoResponse.TokenType,
            ExpiryDate = generateTokenJwtDtoResponse.ExpiryDate
        };
        return Ok(validateLoginApiResponse);
    }

    /// <inheritdoc />
    public override async Task<IActionResult> RecoveryCodesAsync()
    {
        var recoveryCodes = await recoveryCodeService.GenerateRecoveryCodesAsync(Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value));

        return Ok(new GenerateRecoveryCodesApiResponse
        {
            RecoveryCodes = recoveryCodes
        });
    }

    /// <inheritdoc />
    public override async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenApiRequest refreshTokenApiRequest)
    {
        ArgumentNullException.ThrowIfNull(refreshTokenApiRequest, Messages.InvalidRequestBody);
        ArgumentException.ThrowIfNullOrEmpty(refreshTokenApiRequest.Token, Messages.InvalidRequestBody);

        var email = tokenService.ValidateToken(refreshTokenApiRequest.Token); // TODO: Remove when middleware is implemented
        var generateTokenJwtDtoResponse = await tokenService.GenerateTokenJwtAsync(email);

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
