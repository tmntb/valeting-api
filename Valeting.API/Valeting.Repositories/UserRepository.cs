using Valeting.Repository.Models.User;
using Valeting.Repositories.Entities;
using Valeting.Repositories.Interfaces;

namespace Valeting.Repositories;

public class UserRepository(ValetingContext valetingContext) : IUserRepository
{
    public async Task<UserDTO> FindUserByEmail(string username)
    {
        var applicationUser = await valetingContext.ApplicationUsers.FindAsync(username);

        if (applicationUser == null)
            return null;

        return new UserDTO()
        {
            Id = applicationUser.Id,
            Username = username,
            Password = applicationUser.Password,
            Salt = applicationUser.Salt
        };
    }
}