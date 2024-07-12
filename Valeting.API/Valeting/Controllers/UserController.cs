using System.Net;

using Microsoft.AspNetCore.Mvc;

using Valeting.Models.User;
using Valeting.Common.Messages;
using Valeting.Core.Services.Interfaces;
using Valeting.Core.Models.User;
using Valeting.Controllers.BaseController;

namespace Valeting.Controllers;

public class UserController(IUserService userService) : UserBaseController
{
    public override async Task<IActionResult> ValidateLogin([FromBody] ValidateLoginRequest validateLoginRequest)
    {
        try
        {
            var validateLoginSVRequest = new ValidateLoginSVRequest()
            {
                Username = validateLoginRequest.Username,
                Password = validateLoginRequest.Password
            };

            var validateLoginSVResponse = await userService.ValidateLogin(validateLoginSVRequest);
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

            var generateTokenJWTSVRequest = new GenerateTokenJWTSVRequest()
            {
                Username = validateLoginRequest.Username
            };
            var generateTokenJWTSVResponse = await userService.GenerateTokenJWT(generateTokenJWTSVRequest);
            if(generateTokenJWTSVResponse.HasError)
            {
                var userApiError = new UserApiError()
                {
                    Detail = generateTokenJWTSVResponse.Error.Message
                };
                return StatusCode(generateTokenJWTSVResponse.Error.ErrorCode, userApiError);
            }

            var validateLoginResponse = new ValidateLoginResponse()
            {
                Token = generateTokenJWTSVResponse.Token,
                ExpiryDate = generateTokenJWTSVResponse.ExpiryDate,
                TokenType = generateTokenJWTSVResponse.TokenType
            };
            return StatusCode((int)HttpStatusCode.OK, validateLoginResponse);
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