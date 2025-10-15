using Api.Controllers.BaseController;
using Api.Models.User;
using Common.Messages;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using Service.Models.User.Payload;
using System.Net;

namespace Api.Controllers;

public class UserController(IUserService userService) : UserBaseController
{
    public override async Task<IActionResult> Login([FromBody] LoginApiRequest loginApiRequest)
    {
        ArgumentNullException.ThrowIfNull(loginApiRequest, Messages.InvalidRequestBody);

        var validateLoginDtoRequest = new ValidateLoginDtoRequest 
        {
            Username = loginApiRequest.Username,
            Password = loginApiRequest.Password
        };
        var validateLogin = await userService.ValidateLoginAsync(validateLoginDtoRequest);
        if (!validateLogin)
        {
            throw new UnauthorizedAccessException(Messages.InvalidPassword);
        }

        var generateTokenJWTDtoResponse = await userService.GenerateTokenJWTAsync(loginApiRequest.Username);

        var validateLoginApiResponse =  new LoginApiResponse 
        {
            Token = generateTokenJWTDtoResponse.Token,
            TokenType = generateTokenJWTDtoResponse.TokenType,
            ExpiryDate = generateTokenJWTDtoResponse.ExpiryDate
        };
        return StatusCode((int)HttpStatusCode.OK, validateLoginApiResponse);
    }

    public override async Task<IActionResult> Register([FromBody] RegisterApiRequest registerApiRequest)
    {
        ArgumentNullException.ThrowIfNull(registerApiRequest, Messages.InvalidRequestBody);

        var registerDtoRequest = new RegisterDtoRequest 
        {
            Username = registerApiRequest.Username,
            Password = registerApiRequest.Password,
        };
        await userService.RegisterAsync(registerDtoRequest);

        return StatusCode((int)HttpStatusCode.OK);
    }
}