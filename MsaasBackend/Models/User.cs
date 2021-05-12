using System.ComponentModel.DataAnnotations;

namespace MsaasBackend.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public UserDto GetDto() => new UserDto
        {
            Id = Id,
            Username = Username
        };
    }

    public class UserDto
    {
        public int Id { get; set; }

        public string Username { get; set; }
    }

    public class LoginForm
    {
        [Required] public string Username { get; set; }

        [Required] public string Password { get; set; }
    }
}