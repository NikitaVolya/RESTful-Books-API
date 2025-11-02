using System.ComponentModel.DataAnnotations;

namespace RESTful_Books_API.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [MinLength(2)]
        public string FirstName { get; set; } = null!;

        [MinLength(2)]
        public string LastName { get; set; } = null!;

        [EmailAddress]
        public string Email { get; set; } = null!;

        public DateOnly MembershipDate { get; set; }

        public IList<Loan> Loans { get; set; }
    }
}
