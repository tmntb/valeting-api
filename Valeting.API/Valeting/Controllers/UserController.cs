using System.Net;

using Microsoft.AspNetCore.Mvc;

using Valeting.ApiObjects.User;
using Valeting.Common.Exceptions;
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

            if (await userService.ValidateLogin(userDTO))
            {
                var auth = await authenticationService.GenerateTokenJWT(userDTO);
                response.Token = auth.Token;
                response.ExpiryDate = auth.ExpiryDate;
                response.TokenType = auth.TokenType;
            }

            return StatusCode((int)HttpStatusCode.OK, response);
        }
        catch(InputException inputException)
        {
            var userApiError = new UserApiError() 
            { 
                Id = Guid.NewGuid(),
                Detail = inputException.Message
            };
            return StatusCode((int)HttpStatusCode.BadRequest, userApiError);
        }
        catch(NotFoundException notFoundException)
        {
            var userApiError = new UserApiError() 
            { 
                Id = Guid.NewGuid(),
                Detail = notFoundException.Message
            };
            return StatusCode((int)HttpStatusCode.NotFound, userApiError);
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
