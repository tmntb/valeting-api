namespace Api.Models.User.Payload;

public class LoginApiResponse
{
    public string Token { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string TokenType { get; set; }
}
