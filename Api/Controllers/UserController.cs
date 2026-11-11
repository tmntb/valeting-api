using System.Security.Claims;
using Api.Context;
using Api.Controllers.BaseController;
using Api.Models.User.Payload;
using Asp.Versioning;
using Common.Messages;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using Service.Models.User;
using Service.Models.User.Payload;

namespace Api.Controllers;

[ApiVersion("1")]
public class UserController(ValetingApiContext valetingApiContext, IUserService userService) : UserBaseController
{
    /// <inheritdoc />
    public override async Task<IActionResult> UpdateAdminSettingsAsync([FromBody] UpdateAdminSettingsApiRequest updateAdminSettingsApiRequest)
    {
        ArgumentNullException.ThrowIfNull(updateAdminSettingsApiRequest, Messages.InvalidRequestBody);

        var updateAdminSettingsDtoRequest = new UpdateAdminSettingsDtoRequest
        {
            AdminId = valetingApiContext.UserId!.Value,
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
            Id = valetingApiContext.UserId!.Value,
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
            Id = valetingApiContext.UserId!.Value,
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
            Id =valetingApiContext.UserId!.Value,
            FirstName = updateUserApiRequest.FirstName,
            LastName = updateUserApiRequest.LastName,
            DateOfBirth = updateUserApiRequest.DateOfBirth,
            ContactNumber = updateUserApiRequest.ContactNumber,
        };
        await userService.UpdateProfileAsync(updateProfileDtoRequest);

        return NoContent();
    }
}