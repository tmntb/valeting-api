using DJValeting.Business;

namespace DJValeting.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<UserDTO> FindUserByEmail(string email);
    }
}
