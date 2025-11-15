namespace Api.Models.User.Payload;

public class RefreshTokenApiResponse
{
    public string Token { get; set; } = default!;
    public string TokenType { get; set; } = "Bearer";
    public DateTime ExpiryDate { get; set; }
}
