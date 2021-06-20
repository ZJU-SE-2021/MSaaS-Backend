using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MsaasBackend.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Gender
    {
        Male,
        Female,
        Other
    }

    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string PasswordHash { get; set; }

        public string Role { get; set; } = "User";

        public string Name { get; set; }

        public Gender? Gender { get; set; }

        public DateTime? Birthday { get; set; }

        public int? Age
        {
            get
            {
                if (!Birthday.HasValue) return null;
                return DateTime.Today.Year - Birthday.Value.Year;
            }
        }

        public string Phone { get; set; }

        public string Email { get; set; }

        public UserDto ToDto() => new()
        {
            Id = Id,
            Username = Username,
            Name = Name,
            Birthday = Birthday,
            Email = Email,
            Gender = Gender,
            Phone = Phone
        };
    }

    public class UserDto
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string Name { get; set; }

        public Gender? Gender { get; set; }

        [DataType(DataType.Date)] public DateTime? Birthday { get; set; }

        public int? Age
        {
            get
            {
                if (!Birthday.HasValue) return null;
                return DateTime.Today.Year - Birthday.Value.Year;
            }
        }

        public string Phone { get; set; }

        public string Email { get; set; }
    }

    public class LoginForm
    {
        [Required] public string Username { get; set; }

        [Required] public string Password { get; set; }
    }

    public class RegisterForm
    {
        [Required] public string Username { get; set; }

        [Required] public string Password { get; set; }

        public string Name { get; set; }

        public Gender? Gender { get; set; }

        [DataType(DataType.Date)] public DateTime? Birthday { get; set; }

        [Phone] public string Phone { get; set; }

        [EmailAddress] public string Email { get; set; }
    }

    public class UpdateUserForm
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string Name { get; set; }

        public Gender? Gender { get; set; }

        [DataType(DataType.Date)] public DateTime? Birthday { get; set; }

        [Phone] public string Phone { get; set; }

        [EmailAddress] public string Email { get; set; }
    }

    public class UpdateUserFormAdmin : UpdateUserForm
    {
        public string Role { get; set; } = "User";
    }

    public class LoginResult
    {
        public string Token { get; set; }

        public UserDto User { get; set; }
    }
}