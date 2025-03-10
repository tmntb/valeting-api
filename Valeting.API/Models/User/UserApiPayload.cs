using Valeting.API.Models.Core;

namespace Valeting.API.Models.User;

public class ValidateLoginApiRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class ValidateLoginApiResponse
{
    public string Token { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string TokenType { get; set; }
}
