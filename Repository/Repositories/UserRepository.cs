using Microsoft.EntityFrameworkCore;
using Repository.Entities;
using Service.Interfaces;
using Service.Models.User;

namespace Repository.Repositories;

public class UserRepository(ValetingContext valetingContext) : IUserRepository
{
    /// <inheritdoc />
    public async Task<UserDto> GetUserByEmailAsync(string email)
    {
        var applicationUser = await valetingContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Email == email);

        if (applicationUser == null)
            return null;

        return new() 
        {
            Id = applicationUser.Id,
            Username = applicationUser.Username,
            PasswordHash = applicationUser.PasswordHash,
            ContactNumber = applicationUser.ContactNumber,
            Email = applicationUser.Email,
            Role = new()
            {
                Id = applicationUser.Role.Id,
                Name = applicationUser.Role.Name
            },
            IsActive = applicationUser.IsActive,
            CreatedAt = applicationUser.CreatedAt,
            UpdatedAt = applicationUser.UpdatedAt,
            LastLoginAt = applicationUser.LastLoginAt
        };
    }

    /// <inheritdoc />
    public async Task RegisterAsync(UserDto userDto)
    {
        var applicationUser = new ApplicationUser
        {
            Id = userDto.Id,
            Username = userDto.Username,
            PasswordHash = userDto.PasswordHash,
            ContactNumber = userDto.ContactNumber,
            Email = userDto.Email,
            RoleId = userDto.Role.Id,
            IsActive = userDto.IsActive,
            CreatedAt = userDto.CreatedAt,
            UpdatedAt = userDto.UpdatedAt,
            LastLoginAt = userDto.LastLoginAt
        };
        await valetingContext.ApplicationUsers.AddAsync(applicationUser);
        await valetingContext.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task UpdateAsync(UserDto userDto)
    {
        var applicationUser = await valetingContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == userDto.Id);
        if (applicationUser == null)
            return;

        applicationUser.Username = userDto.Username;
        applicationUser.PasswordHash = userDto.PasswordHash;
        applicationUser.ContactNumber = userDto.ContactNumber;
        applicationUser.Email = userDto.Email;
        applicationUser.RoleId = userDto.Role.Id;
        applicationUser.IsActive = userDto.IsActive;
        applicationUser.UpdatedAt = userDto.UpdatedAt;
        applicationUser.LastLoginAt = userDto.LastLoginAt;

        await valetingContext.SaveChangesAsync();   
    }
}