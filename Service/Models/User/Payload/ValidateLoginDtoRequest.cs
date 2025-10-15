namespace Service.Models.User.Payload;

public class ValidateLoginDtoRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}
