using System.Text.Json.Serialization;

namespace Valeting.ApiObjects.User
{
    public class UserApi
    {
        public Guid ID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        [JsonPropertyName("_links")]
        public UserApiLink Links { get; set; }
    }
}
