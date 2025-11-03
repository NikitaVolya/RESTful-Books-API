using System.ComponentModel.DataAnnotations;

namespace RESTful_Books_API.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        [MinLength(4)]
        [MaxLength(100)]
        public string Title { get; set; }

        [MinLength(4)]
        [MaxLength(50)]
        public string Author { get; set; }

        [Range(100000000, 9999999999999)]
        public int ISBN { get; set; }

        [Range(0, 300)]
        public int CopiesAvailable { get; set; }

        public IList<Loan> Loans { get; set; }
    }
}
