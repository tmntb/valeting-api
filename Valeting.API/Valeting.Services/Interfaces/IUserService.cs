using Valeting.Business.Authentication;

namespace Valeting.Services.Interfaces
{
    public interface IUserService
    {
        Task<bool> ValidateLogin(UserDTO userDTO);
    }
}

