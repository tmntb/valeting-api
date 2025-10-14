using Common.Models.User;

namespace Service.Interfaces;

public interface IUserService
{
    Task<ValidateLoginDtoResponse> ValidateLoginAsync(ValidateLoginDtoRequest validateLoginSVRequest);
    Task<GenerateTokenJWTDtoResponse> GenerateTokenJWTAsync(GenerateTokenJWTDtoRequest generateTokenJWTSVRequest);
    Task RegisterAsync(RegisterDtoRequest registerDtoRequest);
}