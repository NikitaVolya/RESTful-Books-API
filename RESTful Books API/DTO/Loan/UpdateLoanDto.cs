namespace RESTful_Books_API.DTO.Loan
{
    public class UpdateLoanDto
    {
        public int UserId { get; set; }
        public int BookId { get; set; }
        public DateOnly LoanDate { get; set; }
        public DateOnly? ReturnDate { get; set; }
    }
}
