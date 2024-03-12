using Valeting.Business.Authentication;

namespace Valeting.Services.Interfaces;

public interface IAuthenticationService
{
    Task<AuthenticationDTO> GenerateTokenJWT(UserDTO userDTO);
}