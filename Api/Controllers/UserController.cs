using System.Security.Claims;
using Api.Controllers.BaseController;
using Api.Models.User.Payload;
using Common.Enums;
using Common.Messages;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using Service.Models.User;
using Service.Models.User.Payload;

namespace Api.Controllers;

public class UserController(IUserService userService) : UserBaseController
{
    /// <inheritdoc />
    public override async Task<IActionResult> LoginAsync([FromBody] LoginApiRequest loginApiRequest)
    {
        ArgumentNullException.ThrowIfNull(loginApiRequest, Messages.InvalidRequestBody);

        var userDto = new UserDto
        {
            Email = loginApiRequest.Email,
            Password = loginApiRequest.Password
        };

        await userService.ValidateLoginAsync(userDto);

        var generateTokenJWTDtoResponse = await userService.GenerateTokenJWTAsync(loginApiRequest.Email);

        var validateLoginApiResponse = new LoginApiResponse
        {
            Token = generateTokenJWTDtoResponse.Token,
            TokenType = generateTokenJWTDtoResponse.TokenType,
            ExpiryDate = generateTokenJWTDtoResponse.ExpiryDate
        };
        return Ok(validateLoginApiResponse);
    }

    /// <inheritdoc />
    public override async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenApiRequest refreshTokenApiRequest)
    {
        ArgumentNullException.ThrowIfNull(refreshTokenApiRequest, Messages.InvalidRequestBody);
        ArgumentException.ThrowIfNullOrEmpty(refreshTokenApiRequest.Token, Messages.InvalidRequestBody);

        var email = userService.ValidateToken(refreshTokenApiRequest.Token);
        var generateTokenJwtDtoResponse = await userService.GenerateTokenJWTAsync(email);

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
            Email = registerApiRequest.Email,
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
        await userService.RegisterAsync(registerDtoRequest);

        return Created();
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
}