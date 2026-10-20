using System.Security.Claims;
using Api.Controllers.BaseController;
using Api.Extensions;
using Api.Models.Auth.Payload;
using Api.Models.WebAuthn.Payload;
using Asp.Versioning;
using Common.Enums;
using Common.Messages;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using Service.Models.User;

namespace Api.Controllers;

[ApiVersion("1")]
public class AuthController(IAuthService authService, IRecoveryCodeService recoveryCodeService, ITokenService tokenService, IWebAuthnService webAuthnService) : AuthBaseController
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

        var generateTokenJwtDtoResponse = await tokenService.GenerateTokenJWTAsync(loginApiRequest.Email);

        //TODO: O login não deve retornar o token.
        //Apenas deve retornar um ok se o login for válido.
        //O token deve ser retornado apenas no endpoint de emitir token.
        //O endpoint de login deve apenas validar o login.
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
        var generateTokenJwtDtoResponse = await tokenService.GenerateTokenJWTAsync(email);

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

    

    /// <inheritdoc />
    public override async Task<IActionResult> PasskeyAuthenticateVerifyAsync([FromBody] PasskeyAuthenticateVerifyApiRequest passkeyAuthenticateVerifyApiRequest)
    {
        ArgumentNullException.ThrowIfNull(passkeyAuthenticateVerifyApiRequest, Messages.InvalidRequestBody);

        var result = await webAuthnService.VerifyAuthenticationAsync(new()
        {
            CeremonyId = passkeyAuthenticateVerifyApiRequest.CeremonyId,
            Credential = passkeyAuthenticateVerifyApiRequest.Credential
        });

        var tokenResponse = await tokenService.GenerateTokenJWTAsync(result.Email);
        return Ok(new LoginApiResponse
        {
            Token = tokenResponse.Token,
            TokenType = tokenResponse.TokenType,
            ExpiryDate = tokenResponse.ExpiryDate
        });
    }

    public override async Task<IActionResult> PasskeysAsync()
    {
        var passkeys = await webAuthnService.GetUserPasskeysAsync(User.GetCurrentUserId());
        return Ok(passkeys.Select(passkey => new PasskeyCredentialApiResponse
        {
            Id = passkey.Id,
            FriendlyName = passkey.FriendlyName,
            CreatedAt = passkey.CreatedAt,
            LastUsedAt = passkey.LastUsedAt
        }));
    }

    public override async Task<IActionResult> RenamePasskeyAsync(Guid id, [FromBody] PasskeyRenameApiRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, Messages.InvalidRequestBody);
        await webAuthnService.RenamePasskeyAsync(User.GetCurrentUserId(), id, request.FriendlyName);
        return NoContent();
    }

    public override async Task<IActionResult> RemovePasskeyAsync(Guid id)
    {
        await webAuthnService.RemovePasskeyAsync(User.GetCurrentUserId(), id);
        return NoContent();
    }
}
