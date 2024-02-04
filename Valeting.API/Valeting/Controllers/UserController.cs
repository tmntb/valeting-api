using System.Net;

using Microsoft.AspNetCore.Mvc;

using Valeting.ApiObjects.User;
using Valeting.Common.Exceptions;
using Valeting.Services.Interfaces;
using Valeting.Business.Authentication;
using Valeting.Controllers.BaseController;

namespace Valeting.Controllers;

public class UserController : UserBaseController
{
    private readonly IUserService _userService;
    private readonly IAuthenticationService _authenticationService;
    private UserApiError _userApiError;

    public UserController(IUserService userService, IAuthenticationService authenticationService)
    {
        _userService = userService;
        _authenticationService = authenticationService;
        _userApiError = new UserApiError() { Id = Guid.NewGuid() };
    }

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

            if (await _userService.ValidateLogin(userDTO))
            {
                var auth = await _authenticationService.GenerateTokenJWT(userDTO);
                response.Token = auth.Token;
                response.ExpiryDate = auth.ExpiryDate;
                response.TokenType = auth.TokenType;
            }

            return StatusCode((int)HttpStatusCode.OK, response);
        }
        catch(InputException inputException)
        {
            _userApiError.Detail = inputException.Message;
            return StatusCode((int)HttpStatusCode.BadRequest, _userApiError);
        }
        catch(NotFoundException notFoundException)
        {
            _userApiError.Detail = notFoundException.Message;
            return StatusCode((int)HttpStatusCode.NotFound, _userApiError);
        }
        catch (Exception ex)
        {
            _userApiError.Detail = ex.Message;
            return StatusCode((int)HttpStatusCode.InternalServerError, _userApiError);
        }
    }
}
