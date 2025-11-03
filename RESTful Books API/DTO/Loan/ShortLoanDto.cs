using System.ComponentModel.DataAnnotations;

namespace RESTful_Books_API.DTO.Loan
{
    public class ShortLoanDto
    {
        public DateOnly LoanDate { get; set; }
        public DateOnly? ReturnDate { get; set; }

        [EmailAddress]
        public string UserEmail { get; set; } = null!;

        [MinLength(4)]
        [MaxLength(100)]
        public string BookTitle { get; set; } = null!;
    }
}
