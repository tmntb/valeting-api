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
            Email = applicationUser.Email,
            PasswordHash = applicationUser.PasswordHash,
            FirstName = applicationUser.FirstName,
            LastName = applicationUser.LastName,
            DateOfBirth = applicationUser.DateOfBirth,
            ContactNumber = applicationUser.ContactNumber,
            Role = new()
            {
                Id = applicationUser.Role.Id,
                Code = applicationUser.Role.Code
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
            Email = userDto.Email,
            PasswordHash = userDto.PasswordHash,
            FirstName = userDto.FirstName,
            LastName = userDto.LastName,
            DateOfBirth = userDto.DateOfBirth,
            ContactNumber = userDto.ContactNumber,
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

        applicationUser.Email = userDto.Email;
        applicationUser.PasswordHash = userDto.PasswordHash;
        applicationUser.FirstName = userDto.FirstName;
        applicationUser.LastName = userDto.LastName;
        applicationUser.DateOfBirth = userDto.DateOfBirth;
        applicationUser.ContactNumber = userDto.ContactNumber;
        applicationUser.RoleId = userDto.Role.Id;
        applicationUser.IsActive = userDto.IsActive;
        applicationUser.UpdatedAt = userDto.UpdatedAt;
        applicationUser.LastLoginAt = userDto.LastLoginAt;

        await valetingContext.SaveChangesAsync();   
    }
}