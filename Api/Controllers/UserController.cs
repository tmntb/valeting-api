using Microsoft.AspNetCore.Mvc;
using System.Net;
using Api.Controllers.BaseController;
using Api.Models.User;
using Common.Messages;
using Common.Models.User;
using Service.Interfaces;

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
        var validateLoginDtoResponse = await userService.ValidateLoginAsync(validateLoginDtoRequest);
        if (!validateLoginDtoResponse.Valid)
        {
            throw new UnauthorizedAccessException(Messages.InvalidPassword);
        }

        var generateTokenJWTDtoRequest = new GenerateTokenJWTDtoRequest { };
        var generateTokenJWTDtoResponse = await userService.GenerateTokenJWTAsync(generateTokenJWTDtoRequest);

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