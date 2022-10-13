using System.Net;

using Microsoft.AspNetCore.Mvc;

using Valeting.Business;
using Valeting.ApiObjects;
using Valeting.Controllers.BaseController;
using Valeting.Services.Interfaces;

namespace Valeting.Controllers
{
    public class UserController : UserBaseController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        public override async Task<IActionResult> ValidateLogin([FromBody] UserApi userApi)
        {
            try
            {
                var userDTO = new UserDTO()
                {
                    Username = userApi.Username,
                    Password = userApi.Password
                };

                bool validLogin = await _userService.ValidateLogin(userDTO);

                return StatusCode((int)HttpStatusCode.OK, validLogin);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
