using Common.Models.User;

namespace Service.Interfaces;

public interface IUserService
{
    Task<GenerateTokenJWTDtoResponse> GenerateTokenJWTAsync(string username);
    Task RegisterAsync(RegisterDtoRequest registerDtoRequest);
    Task<bool> ValidateLoginAsync(ValidateLoginDtoRequest validateLoginSVRequest);
}