namespace Service.Models.User.Payload;

public class GenerateTokenJWTDtoResponse
{
    public string Token { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string TokenType { get; set; }
}
