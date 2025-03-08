using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Valeting.API.Models.User;
using Valeting.Common.Messages;
using Valeting.Core.Interfaces;
using Valeting.Common.Models.User;
using Valeting.API.Controllers.BaseController;

namespace Valeting.API.Controllers;

public class UserController(IUserService userService, IMapper mapper) : UserBaseController
{
    public override async Task<IActionResult> ValidateLogin([FromBody] ValidateLoginApiRequest validateLoginApiRequest)
    {
        ArgumentNullException.ThrowIfNull(validateLoginApiRequest, "Invalid request body");

        var validateLoginDtoRequest = mapper.Map<ValidateLoginDtoRequest>(validateLoginApiRequest);
        var validateLoginDtoResponse = await userService.ValidateLoginAsync(validateLoginDtoRequest);
        if (!validateLoginDtoResponse.Valid)
        {
            throw new UnauthorizedAccessException(Messages.InvalidPassword);
        }

        var generateTokenJWTDtoRequest = mapper.Map<GenerateTokenJWTDtoRequest>(validateLoginApiRequest);
        var generateTokenJWTDtoResponse = await userService.GenerateTokenJWTAsync(generateTokenJWTDtoRequest);

        var validateLoginApiResponse = mapper.Map<ValidateLoginApiResponse>(generateTokenJWTDtoResponse);
        return StatusCode((int)HttpStatusCode.OK, validateLoginApiResponse);
    }
}