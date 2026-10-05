using System.Security.Claims;
using Api.Controllers.BaseController;
using Api.Models.Auth.Payload;
using Api.Models.User.Payload;
using Common.Messages;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using Service.Models.User;
using Service.Models.User.Payload;

namespace Api.Controllers;

[ApiController]
public class UserController(IUserService userService) : UserBaseController
{
    /// <inheritdoc />
    public override async Task<IActionResult> ForgotPasswordAsync([FromBody] ForgotPasswordApiRequest forgotPasswordApiRequest)
    {
        ArgumentNullException.ThrowIfNull(forgotPasswordApiRequest, Messages.InvalidRequestBody);

        await userService.ForgotPasswordAsync(forgotPasswordApiRequest.Email);

        return Ok();
    }

    /// <inheritdoc />
    public override async Task<IActionResult> ResetAsync([FromBody] ResetApiRequest resetApiRequest)
    {
        ArgumentNullException.ThrowIfNull(resetApiRequest, Messages.InvalidRequestBody);

        var userDto = new UserDto
        {
            Email = resetApiRequest.Email,
            Password = resetApiRequest.NewPassword
        };
        await userService.ResetAsync(userDto);

        return NoContent();
    }

    /// <inheritdoc />
    public override async Task<IActionResult> UpdateAdminSettingsAsync([FromBody] UpdateAdminSettingsApiRequest updateAdminSettingsApiRequest)
    {
        ArgumentNullException.ThrowIfNull(updateAdminSettingsApiRequest, Messages.InvalidRequestBody);

        var updateAdminSettingsDtoRequest = new UpdateAdminSettingsDtoRequest
        {
            AdminId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value),
            UserId = updateAdminSettingsApiRequest.UserId,
            IsActive = updateAdminSettingsApiRequest.IsActive,
            RoleId = updateAdminSettingsApiRequest.RoleId
        };
        await userService.UpdateAdminSettingsAsync(updateAdminSettingsDtoRequest);

        return NoContent();
    }

    /// <inheritdoc />
    public override async Task<IActionResult> UpdateEmailAsync([FromBody] UpdateEmailApiRequest updateEmailApiRequest)
    {
        ArgumentNullException.ThrowIfNull(updateEmailApiRequest, Messages.InvalidRequestBody);

        var userDto = new UserDto
        {
            Id = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value),
            Email = updateEmailApiRequest.Email
        };
        await userService.UpdateEmailAsync(userDto);

        return NoContent();
    }

    /// <inheritdoc />
    public override async Task<IActionResult> UpdatePasswordAsync([FromBody] UpdatePasswordApiRequest updatePasswordApiRequest)
    {
        ArgumentNullException.ThrowIfNull(updatePasswordApiRequest, Messages.InvalidRequestBody);

        var userDto = new UserDto
        {
            Id = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value),
            Password = updatePasswordApiRequest.Password
        };
        await userService.UpdatePasswordAsync(userDto);

        return NoContent();
    }

    /// <inheritdoc />
    public override async Task<IActionResult> UpdateProfileAsync([FromBody] UpdateProfileApiRequest updateUserApiRequest)
    {
        ArgumentNullException.ThrowIfNull(updateUserApiRequest, Messages.InvalidRequestBody);

        var updateProfileDtoRequest = new UpdateProfileDtoRequest
        {
            Id = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value),
            FirstName = updateUserApiRequest.FirstName,
            LastName = updateUserApiRequest.LastName,
            DateOfBirth = updateUserApiRequest.DateOfBirth,
            ContactNumber = updateUserApiRequest.ContactNumber,
        };
        await userService.UpdateProfileAsync(updateProfileDtoRequest);

        return NoContent();
    }
}