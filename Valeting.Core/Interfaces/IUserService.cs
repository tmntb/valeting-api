using Valeting.Common.Models.User;

namespace Valeting.Core.Interfaces;

public interface IUserService
{
    Task<ValidateLoginDtoResponse> ValidateLoginAsync(ValidateLoginDtoRequest validateLoginSVRequest);
    Task<GenerateTokenJWTDtoResponse> GenerateTokenJWTAsync(GenerateTokenJWTDtoRequest generateTokenJWTSVRequest);
}