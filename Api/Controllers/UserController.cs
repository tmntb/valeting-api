using Api.Controllers.BaseController;
using Api.Models.User.Payload;
using Common.Messages;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using Service.Models.User.Payload;
using System.Net;

namespace Api.Controllers;

public class UserController(IUserService userService) : UserBaseController
{
    /// <inheritdoc />
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

    /// <inheritdoc />
    public override async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenApiRequest refreshTokenApiRequest)
    {
        ArgumentNullException.ThrowIfNull(refreshTokenApiRequest, Messages.InvalidRequestBody);
        ArgumentException.ThrowIfNullOrEmpty(refreshTokenApiRequest.Token, Messages.InvalidRequestBody);

        var username = userService.ValidateToken(refreshTokenApiRequest.Token);
        var generateTokenJwtDtoResponse = await userService.GenerateTokenJWTAsync(username);

        var refreshTokenApiResponse = new RefreshTokenApiResponse
        {
            Token = generateTokenJwtDtoResponse.Token,
            TokenType = generateTokenJwtDtoResponse.TokenType,
            ExpiryDate = generateTokenJwtDtoResponse.ExpiryDate
        };

        return StatusCode((int)HttpStatusCode.OK, refreshTokenApiResponse);
    }

    /// <inheritdoc />
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