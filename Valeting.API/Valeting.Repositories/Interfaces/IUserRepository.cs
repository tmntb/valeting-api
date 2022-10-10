using Valeting.Business;

namespace Valeting.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<UserDTO> FindUserByEmail(string email);
    }
}
