using System.ComponentModel.DataAnnotations;

namespace RESTful_Books_API.DTO.User
{
    public class LoginUserDto
    {
        [EmailAddress]
        public string Email { get; set; } = null!;

        [MinLength(6)]
        public string Password { get; set; } = null!;
    }
}
