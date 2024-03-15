using Valeting.Business.Authentication;

namespace Valeting.Services.Interfaces;

public interface IUserService
{
    Task<LoginDTO> ValidateLogin(UserDTO userDTO);
}