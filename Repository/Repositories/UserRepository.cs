using Repository.Entities;
using Service.Interfaces;
using Service.Models.User;

namespace Repository.Repositories;

public class UserRepository(ValetingContext valetingContext) : IUserRepository
{
    public async Task<UserDto> GetUserByEmailAsync(string username)
    {
        var applicationUser = await valetingContext.ApplicationUsers.FindAsync(username);

        if (applicationUser == null)
            return null;

        return new() 
        {
            Id = applicationUser.Id,
            Username = applicationUser.Username,
            Password = applicationUser.Password
        };
    }

    public async Task RegisterAsync(UserDto userDto)
    {
        var applicationUser = new ApplicationUser
        {
            Id = userDto.Id,
            Username = userDto.Username,
            Password = userDto.Password
        };
        await valetingContext.ApplicationUsers.AddAsync(applicationUser);
        await valetingContext.SaveChangesAsync();
    }
}