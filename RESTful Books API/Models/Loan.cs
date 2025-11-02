using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RESTful_Books_API.Models
{
    public class Loan
    {
        [Key]
        public int Id { get; set; }
        public DateOnly LoanDate { get; set; }
        public DateOnly? ReturnDate { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }

        [ForeignKey("Book")]
        public int BookId { get; set; }
        public Book Book { get; set; }
    }
}
