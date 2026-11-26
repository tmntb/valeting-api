using Api.Controllers.v1.BaseController;
using Api.Models.Auth.Payload;
using Asp.Versioning;
using Common.Enums;
using Common.Messages;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using Service.Models.User;

namespace Api.Controllers.v1;

[ApiVersion("1")]
public class AuthController(IAuthService authService) : AuthBaseController
{
    /// <inheritdoc />
    public override async Task<IActionResult> ForgotPasswordAsync([FromBody] ForgotPasswordApiRequest forgotPasswordApiRequest)
    {
        ArgumentNullException.ThrowIfNull(forgotPasswordApiRequest, Messages.InvalidRequestBody);

        await authService.ForgotPasswordAsync(new()
        {
            Email = forgotPasswordApiRequest.Email
        });

        return Ok();
    }

    /// <inheritdoc />
    public override async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordApiRequest resetPasswordApiRequest)
    {
        ArgumentNullException.ThrowIfNull(resetPasswordApiRequest, Messages.InvalidRequestBody);

        await authService.ResetPasswordAsync(new()
        {
            Token = resetPasswordApiRequest.Token,
            NewPassword = resetPasswordApiRequest.NewPassword
        });

        return NoContent();
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

        return Ok();
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
