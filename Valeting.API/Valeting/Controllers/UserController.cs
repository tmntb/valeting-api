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

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        public override async Task<IActionResult> ValidateLogin([FromBody] ValidateLoginRequest validateLoginRequest)
        {
            try
            {
                var response = new ValidateLoginResponse()
                {
                    Token = string.Empty,
                    Sucess = false
                };

                var userDTO = new UserDTO()
                {
                    Username = validateLoginRequest.Username,
                    Password = validateLoginRequest.Password
                };

                response.Sucess = await _userService.ValidateLogin(userDTO);

                return StatusCode((int)HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
