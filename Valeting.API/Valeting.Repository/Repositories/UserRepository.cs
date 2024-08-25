using AutoMapper;

using Valeting.Repository.Entities;
using Valeting.Repository.Models.User;
using Valeting.Repository.Repositories.Interfaces;

namespace Valeting.Repository.Repositories;

public class UserRepository(ValetingContext valetingContext, IMapper mapper) : IUserRepository
{
    public async Task<UserDTO> FindUserByEmailAsync(string username)
    {
        var applicationUser = await valetingContext.ApplicationUsers.FindAsync(username);

        if (applicationUser == null)
            return null;

        return mapper.Map<UserDTO>(applicationUser);
    }
}