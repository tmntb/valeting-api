using System.Net;

using Microsoft.AspNetCore.Mvc;

using Valeting.ApiObjects.User;
using Valeting.Services.Interfaces;
using Valeting.Business.Authentication;
using Valeting.Controllers.BaseController;

namespace Valeting.Controllers;

public class UserController(IUserService userService, IAuthenticationService authenticationService) : UserBaseController
{
    public override async Task<IActionResult> ValidateLogin([FromBody] ValidateLoginRequest validateLoginRequest)
    {
        try
        {
            var response = new ValidateLoginResponse()
            {
                Token = string.Empty,
                ExpiryDate = DateTime.MinValue,
                TokenType = string.Empty
            };

            var userDTO = new UserDTO()
            {
                Username = validateLoginRequest.Username,
                Password = validateLoginRequest.Password
            };

            var login = await userService.ValidateLogin(userDTO);
            if(login.Errors.Any())
            {
                var error = login.Errors.FirstOrDefault();
                var userApiError = new UserApiError()
                {
                    Id = error.Id,
                    Detail = error.Detail
                };
                return StatusCode(error.ErrorCode, userApiError);
            }

            if (login.Valid)
            {
                var auth = await authenticationService.GenerateTokenJWT(userDTO);
                if(auth.Errors.Any())
                {
                    var error = auth.Errors.FirstOrDefault();
                    var userApiError = new UserApiError() 
                    { 
                        Id = error.Id,
                        Detail = error.Detail
                    };
                    return StatusCode(error.ErrorCode, userApiError);
                }

                response.Token = auth.Token;
                response.ExpiryDate = auth.ExpiryDate;
                response.TokenType = auth.TokenType;
            }

            return StatusCode((int)HttpStatusCode.OK, response);
        }
        catch (Exception ex)
        {
            var userApiError = new UserApiError() 
            { 
                Id = Guid.NewGuid(),
                Detail = ex.Message
            };
            return StatusCode((int)HttpStatusCode.InternalServerError, userApiError);
        }
    }
}