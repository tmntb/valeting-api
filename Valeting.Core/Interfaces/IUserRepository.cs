using Valeting.Common.Models.User;

namespace Valeting.Repository.Interfaces;

public interface IUserRepository
{
    Task<UserDto> GetUserByEmailAsync(string email);
    Task RegisterAsync(UserDto userDto);
}