using Valeting.Common.Models.Core;

namespace Valeting.Common.Models.User;

public class ValidateLoginDtoRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class ValidateLoginDtoResponse : ValetingOutputDto 
{
    public bool Valid { get; set; }
}

public class GenerateTokenJWTDtoRequest
{
    public string Username { get; set; }
}

public class GenerateTokenJWTDtoResponse : ValetingOutputDto
{
    public string Token { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string TokenType { get; set; }
}

public class RegisterDtoRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}