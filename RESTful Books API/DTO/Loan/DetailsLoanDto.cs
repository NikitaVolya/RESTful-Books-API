namespace RESTful_Books_API.DTO.Loan
{
    public class DetailsLoanDto
    {
        public class UserData
        {
            public int Id { get; set; }
            public string FullName { get; set; } = null!;
            public string FirstName { get; set; } = null!;
            public string LastName { get; set; } = null!;
            public string Email { get; set; } = null!;
            public DateOnly MembershipDate { get; set; }
        }
        public class BookData
        {
            public int Id { get; set; }
            public string Title { get; set; } = null!;
            public string Author { get; set; } = null!;
            public int ISBN { get; set; }
        }

        public int Id { get; set; }
        public DateOnly LoanDate { get; set; }
        public DateOnly? ReturnDate { get; set; }

        public UserData User { get; set; } = null!;
        public BookData Book { get; set; } = null!;
    }
}
