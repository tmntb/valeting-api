using System.Net;

using Microsoft.AspNetCore.Mvc;

using DJValeting.Service;
using DJValeting.Business;
using DJValeting.ApiObjects;
using DJValeting.Repositories;
using DJValeting.Controllers.BaseController;

namespace DJValeting.Controllers
{
    public class UserController : UserBaseController
    {
        private readonly IConfiguration _configuration;

        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public override async Task<IActionResult> ValidateLogin([FromBody] UserApi userApi)
        {
            try
            {
                UserDTO userDTO = new()
                {
                    Username = userApi.Username,
                    Password = userApi.Password
                };

                UserService userService = new(new UserRepository(_configuration));
                bool validLogin = await userService.ValidateLogin(userDTO);

                return StatusCode((int)HttpStatusCode.OK, validLogin);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
