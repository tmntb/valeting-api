﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Valeting.API.Controllers.BaseController;
using Valeting.API.Models.User;
using Valeting.Common.Messages;
using Valeting.Common.Models.User;
using Valeting.Core.Interfaces;

namespace Valeting.API.Controllers;

public class UserController(IUserService userService, IMapper mapper) : UserBaseController
{
    public override async Task<IActionResult> Login([FromBody] LoginApiRequest loginApiRequest)
    {
        ArgumentNullException.ThrowIfNull(loginApiRequest, Messages.InvalidRequestBody);

        var validateLoginDtoRequest = mapper.Map<ValidateLoginDtoRequest>(loginApiRequest);
        var validateLoginDtoResponse = await userService.ValidateLoginAsync(validateLoginDtoRequest);
        if (!validateLoginDtoResponse.Valid)
        {
            throw new UnauthorizedAccessException(Messages.InvalidPassword);
        }

        var generateTokenJWTDtoRequest = mapper.Map<GenerateTokenJWTDtoRequest>(loginApiRequest);
        var generateTokenJWTDtoResponse = await userService.GenerateTokenJWTAsync(generateTokenJWTDtoRequest);

        var validateLoginApiResponse = mapper.Map<LoginApiResponse>(generateTokenJWTDtoResponse);
        return StatusCode((int)HttpStatusCode.OK, validateLoginApiResponse);
    }

    public override async Task<IActionResult> Register([FromBody] RegisterApiRequest registerApiRequest)
    {
        ArgumentNullException.ThrowIfNull(registerApiRequest, Messages.InvalidRequestBody);

        var registerDtoRequest = mapper.Map<RegisterDtoRequest>(registerApiRequest);
        await userService.RegisterAsync(registerDtoRequest);

        return StatusCode((int)HttpStatusCode.OK);
    }
}