using Valeting.Common.Models.User;

namespace Valeting.Core.Interfaces;

public interface IUserRepository
{
    Task<UserDto> GetUserByEmailAsync(string email);
    Task RegisterAsync(UserDto userDto);
}