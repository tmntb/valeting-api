using Valeting.Services.Objects.Core;

namespace Valeting.Services.Objects.User;

public class ValidateLoginSVRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class ValidateLoginSVResponse : ValetingOutputSV 
{
    public bool Valid { get; set; }
}

public class GenerateTokenJWTSVRequest
{
    public string Username { get; set; }
}

public class GenerateTokenJWTSVResponse : ValetingOutputSV
{
    public string Token { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string TokenType { get; set; }
}