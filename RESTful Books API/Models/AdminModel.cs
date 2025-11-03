using System.ComponentModel.DataAnnotations;

namespace RESTful_Books_API.Models
{
    public class AdminModel
    {
        public int Id { get; set; }

        [MaxLength(50)]
        public string Username { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
    }
}
