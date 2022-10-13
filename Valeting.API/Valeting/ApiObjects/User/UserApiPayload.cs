namespace Valeting.ApiObjects.User
{
    public class ValidateLoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class ValidateLoginResponse
    {
        public string Token { get; set; }
        public bool Sucess { get; set; }
    }

    public class UserApiError : ErrorApi { }
}

