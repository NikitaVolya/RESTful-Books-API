namespace RESTful_Books_API.DTO.User
{
    public class DetailsUserDto
    {
        public class LoanData
        {
            public int Id { get; set; }
            public int BookId { get; set; }
            public string BookTitle { get; set; } = null!;
            public DateOnly LoanDate { get; set; }
            public DateOnly? ReturnDate { get; set; }
        }

        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateOnly MembershipDate { get; set; }

        public IList<LoanData> CurrentLoans { get; set; }
        public IList<LoanData> PastLoans { get; set; }

    }
}
