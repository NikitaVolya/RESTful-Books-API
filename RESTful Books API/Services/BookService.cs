using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RESTful_Books_API.Data;


namespace RESTful_Books_API.Services
{
    public class BookService
    {
        private readonly AppDbContext _context;

        public BookService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetRelevantAvailableCopiesAsync(int bookId)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null)
            {
                throw new ArgumentException("Book not found.");
            }

            int relevantLoansNumber = await _context.Loans
                .Where(l => l.BookId == bookId && l.ReturnDate == null)
                .CountAsync();

            return book.CopiesAvailable - relevantLoansNumber;
        }

        public int GetRelevantAvailableCopies(int bookId)
        {
            var book = _context.Books.Find(bookId);
            if (book == null)
            {
                throw new ArgumentException("Book not found.");
            }

            int relevantLoansNumber = _context.Loans
                .Where(l => l.BookId == bookId && l.ReturnDate == null)
                .Count();

            return book.CopiesAvailable - relevantLoansNumber;
        }
    }
}
