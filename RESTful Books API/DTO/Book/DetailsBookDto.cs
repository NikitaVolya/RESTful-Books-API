using System.ComponentModel.DataAnnotations;

namespace RESTful_Books_API.DTO.Book
{
    public class DetailsBookDto
    {
        public class LoanData {
            public int Id { get; set; }
            public int UserId { get; set; }
            public string UserFullName { get; set; } = null!;
            public string UserFirstName { get; set; } = null!;
            public string UserLastName { get; set; } = null!;
            public string UserEmail { get; set; } = null!;
            public DateOnly LoanDate { get; set; }
            public DateOnly? ReturnDate { get; set; }
        }

        [Key]
        public int Id { get; set; }

        [MinLength(4)]
        [MaxLength(100)]
        public string Title { get; set; } = null!;

        [MinLength(4)]
        [MaxLength(50)]
        public string Author { get; set; } = null!;

        [Range(9, 9)]
        public int ISBN { get; set; }

        [Range(0, 300)]
        public int CopiesAvailable { get; set; }

        [Range(0, 300)]
        public int CopiesInStock { get; set; }

        public IList<LoanData> CurrentLoans { get; set; } = null!;
        public IList<LoanData> PastLoans { get; set; } = null!;

    }
}
