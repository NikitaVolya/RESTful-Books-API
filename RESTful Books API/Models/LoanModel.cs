using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RESTful_Books_API.Models
{
    public class LoanModel
    {
        [Key]
        public int Id { get; set; }
        public DateOnly LoanDate { get; set; }
        public DateOnly? ReturnDate { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public UserModel User { get; set; } = null!;

        [ForeignKey("Book")]
        public int BookId { get; set; }
        public BookModel Book { get; set; } = null!;
    }
}
