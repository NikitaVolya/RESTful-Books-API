using System.ComponentModel.DataAnnotations;

namespace RESTful_Books_API.DTO.User
{
    public class ChangePasswordDto
    {
        [MinLength(6)]
        public string CurrentPassword { get; set; } = null!;


        [MinLength(6)]
        public string NewPassword { get; set; } = null!;
    }
}
