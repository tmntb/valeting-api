using System.ComponentModel.DataAnnotations;

namespace DJValeting.Models
{
    public class User
    {
        [DataType(DataType.EmailAddress)]
        [Required]
        public string Username { get; set; }
        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }
    }
}
