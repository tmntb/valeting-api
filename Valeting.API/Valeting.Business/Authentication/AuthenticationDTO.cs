namespace Valeting.Business.Authentication;

public class AuthenticationDTO
{
    public string Token { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string TokenType { get; set; }

    public List<ErrorDTO> Errors { get; set; }
}