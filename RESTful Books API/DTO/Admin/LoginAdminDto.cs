using System.ComponentModel.DataAnnotations;


namespace RESTful_Books_API.DTO.Admin
{
    public class LoginAdminDto
    {
        public string Username { get; set; } = null!;

        [MinLength(8)]
        public string Password { get; set; } = null!;
    }
}
