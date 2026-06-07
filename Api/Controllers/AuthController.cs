using Api.Controllers.BaseController;
using Api.Models.Auth.Payload;
using Common.Enums;
using Common.Messages;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using Service.Models.User;

namespace Api.Controllers;

public class AuthController(IAuthService authService) : AuthBaseController
{
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
