using AutoMapper;
using Valeting.Common.Models.User;
using Valeting.Repository.Entities;
using Valeting.Repository.Interfaces;

namespace Valeting.Repository.Repositories;

public class UserRepository(ValetingContext valetingContext, IMapper mapper) : IUserRepository
{
    public async Task<UserDto> GetUserByEmailAsync(string username)
    {
        var applicationUser = await valetingContext.ApplicationUsers.FindAsync(username);

        if (applicationUser == null)
            return null;

        return mapper.Map<UserDto>(applicationUser);
    }

    public async Task RegisterAsync(UserDto userDto)
    {
        var applicationUser = mapper.Map<ApplicationUser>(userDto);
        await valetingContext.ApplicationUsers.AddAsync(applicationUser);
        await valetingContext.SaveChangesAsync();
    }
}