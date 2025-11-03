using System.ComponentModel.DataAnnotations;

namespace RESTful_Books_API.Models
{
    public class UserModel
    {
        [Key]
        public int Id { get; set; }

        [MinLength(2)]
        public string FirstName { get; set; } = null!;

        [MinLength(2)]
        public string LastName { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        [EmailAddress]
        public string Email { get; set; } = null!;

        public DateOnly MembershipDate { get; set; }

        public IList<LoanModel> Loans { get; set; }
    }
}
