using Valeting.Business.Core;

namespace Valeting.Business.Authentication;

public class UserDTO
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Salt { get; set; }
    public List<ErrorDTO> Errors { get; set; }
}

public class AuthenticationDTO
{
    public string Token { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string TokenType { get; set; }
    public List<ErrorDTO> Errors { get; set; }
}

public class LoginDTO
{
    public bool Valid { get; set; }
    public List<ErrorDTO> Errors { get; set; }
}