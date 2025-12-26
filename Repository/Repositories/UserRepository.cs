using Microsoft.EntityFrameworkCore;
using Repository.Entities;
using Service.Interfaces;
using Service.Models.User;

namespace Repository.Repositories;

public class UserRepository(ValetingContext valetingContext) : IUserRepository
{
    /// <inheritdoc />
    public async Task<UserDto> GetUserByEmailAsync(string username)
    {
        var applicationUser = await valetingContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Username == username);

        if (applicationUser == null)
            return null;

        return new() 
        {
            Id = applicationUser.Id,
            Username = applicationUser.Username,
            Password = applicationUser.Password,
            Role = new()
            {
                Id = applicationUser.Role.Id,
                Name = applicationUser.Role.Name
            },
            IsActive = applicationUser.IsActive
        };
    }

    /// <inheritdoc />
    public async Task RegisterAsync(UserDto userDto)
    {
        var applicationUser = new ApplicationUser
        {
            Id = userDto.Id,
            Username = userDto.Username,
            Password = userDto.Password,
            RoleId = userDto.Role.Id,
            IsActive = userDto.IsActive
        };
        await valetingContext.ApplicationUsers.AddAsync(applicationUser);
        await valetingContext.SaveChangesAsync();
    }
}