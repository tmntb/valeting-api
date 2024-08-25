using AutoMapper;

using Microsoft.AspNetCore.Mvc;

using System.Net;

using Valeting.Models.User;
using Valeting.Common.Messages;
using Valeting.Core.Models.User;
using Valeting.Core.Services.Interfaces;
using Valeting.Controllers.BaseController;

namespace Valeting.Controllers;

public class UserController(IUserService userService, IMapper mapper) : UserBaseController
{
    public override async Task<IActionResult> ValidateLogin([FromBody] ValidateLoginApiRequest validateLoginApiRequest)
    {
        try
        {
            var validateLoginSVRequest = mapper.Map<ValidateLoginSVRequest>(validateLoginApiRequest);

            var validateLoginSVResponse = await userService.ValidateLoginAsync(validateLoginSVRequest);
            if(validateLoginSVResponse.HasError)
            {
                var userApiError = new UserApiError()
                {
                    Detail = validateLoginSVResponse.Error.Message
                };
                return StatusCode(validateLoginSVResponse.Error.ErrorCode, userApiError);
            }

            if (!validateLoginSVResponse.Valid)
            {
                var userApiError = new UserApiError()
                {
                    Detail = Messages.InvalidPassword
                };
                return StatusCode((int)HttpStatusCode.Unauthorized, userApiError);
            }

            var generateTokenJWTSVRequest = mapper.Map<GenerateTokenJWTSVRequest>(validateLoginApiRequest);
            var generateTokenJWTSVResponse = await userService.GenerateTokenJWTAsync(generateTokenJWTSVRequest);
            if(generateTokenJWTSVResponse.HasError)
            {
                var userApiError = new UserApiError()
                {
                    Detail = generateTokenJWTSVResponse.Error.Message
                };
                return StatusCode(generateTokenJWTSVResponse.Error.ErrorCode, userApiError);
            }

            var validateLoginApiResponse = mapper.Map<ValidateLoginApiResponse>(generateTokenJWTSVResponse);
            return StatusCode((int)HttpStatusCode.OK, validateLoginApiResponse);
        }
        catch (Exception ex)
        {
            var userApiError = new UserApiError() 
            { 
                Detail = ex.StackTrace
            };
            return StatusCode((int)HttpStatusCode.InternalServerError, userApiError);
        }
    }
}