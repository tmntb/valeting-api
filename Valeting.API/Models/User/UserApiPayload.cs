namespace Valeting.API.Models.User;

public class LoginApiRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class LoginApiResponse
{
    public string Token { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string TokenType { get; set; }
}

public class RegisterApiRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}