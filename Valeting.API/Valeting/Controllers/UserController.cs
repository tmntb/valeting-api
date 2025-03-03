using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Valeting.Models.User;
using Valeting.Common.Messages;
using Valeting.Core.Interfaces;
using Valeting.Common.Models.User;
using Valeting.Controllers.BaseController;

namespace Valeting.Controllers;

public class UserController(IUserService userService, IMapper mapper) : UserBaseController
{
    public override async Task<IActionResult> ValidateLogin([FromBody] ValidateLoginApiRequest validateLoginApiRequest)
    {
        try
        {
            var validateLoginDtoRequest = mapper.Map<ValidateLoginDtoRequest>(validateLoginApiRequest);

            var validateLoginDtoResponse = await userService.ValidateLoginAsync(validateLoginDtoRequest);
            if(validateLoginDtoResponse.HasError)
            {
                var userApiError = new UserApiError
                {
                    Detail = validateLoginDtoResponse.Error.Message
                };
                return StatusCode(validateLoginDtoResponse.Error.ErrorCode, userApiError);
            }

            if (!validateLoginDtoResponse.Valid)
            {
                var userApiError = new UserApiError
                {
                    Detail = Messages.InvalidPassword
                };
                return StatusCode((int)HttpStatusCode.Unauthorized, userApiError);
            }

            var generateTokenJWTDtoRequest = mapper.Map<GenerateTokenJWTDtoRequest>(validateLoginApiRequest);
            var generateTokenJWTDtoResponse = await userService.GenerateTokenJWTAsync(generateTokenJWTDtoRequest);
            if(generateTokenJWTDtoResponse.HasError)
            {
                var userApiError = new UserApiError
                {
                    Detail = generateTokenJWTDtoResponse.Error.Message
                };
                return StatusCode(generateTokenJWTDtoResponse.Error.ErrorCode, userApiError);
            }

            var validateLoginApiResponse = mapper.Map<ValidateLoginApiResponse>(generateTokenJWTDtoResponse);
            return StatusCode((int)HttpStatusCode.OK, validateLoginApiResponse);
        }
        catch (Exception ex)
        {
            var userApiError = new UserApiError
            { 
                Detail = ex.Message
            };
            return StatusCode((int)HttpStatusCode.InternalServerError, userApiError);
        }
    }
}