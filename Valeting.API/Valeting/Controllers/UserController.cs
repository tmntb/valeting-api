using System.Net;

using Microsoft.AspNetCore.Mvc;

using Valeting.Business;
using Valeting.ApiObjects;
using Valeting.Controllers.BaseController;
using Valeting.Services.Interfaces;
using Valeting.ApiObjects.User;

namespace Valeting.Controllers
{
    public class UserController : UserBaseController
    {
        private readonly IUserService _userService;
        private readonly IAuthenticationService _authenticationService;

        public UserController(IUserService userService, IAuthenticationService authenticationService)
        {
            _userService = userService;
            _authenticationService = authenticationService;
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
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
