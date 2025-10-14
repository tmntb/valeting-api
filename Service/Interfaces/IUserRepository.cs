using Common.Models.User;

namespace Service.Interfaces;

public interface IUserRepository
{
    Task<UserDto> GetUserByEmailAsync(string email);
    Task RegisterAsync(UserDto userDto);
}