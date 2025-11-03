using System.ComponentModel.DataAnnotations;

namespace RESTful_Books_API.DTO.User
{
    public class CreateUserDto
    {
        [MinLength(2)]
        [MaxLength(50)]
        public string FirstName { get; set; } = null!;

        [MinLength(2)]
        [MaxLength(50)]
        public string LastName { get; set; } = null!;

        [EmailAddress]
        public string Email { get; set; } = null!;

        [MinLength(6)]
        public string Password { get; set; } = null!;
    }
}
