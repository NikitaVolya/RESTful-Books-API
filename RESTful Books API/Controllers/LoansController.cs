using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RESTful_Books_API.Data;
using RESTful_Books_API.DTO.Loan;
using RESTful_Books_API.Models;
using RESTful_Books_API.Services;
using System.Security.Claims;

namespace RESTful_Books_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoansController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly BookService _bookService;

        public LoansController(AppDbContext context, IMapper mapper, BookService bookService)
        {
            _context = context;
            _mapper = mapper;
            _bookService = bookService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id, [FromQuery] bool details = false)
        {
            var loan = await _context.Loans
                    .Include(l => l.User)
                    .Include(l => l.Book)
                    .FirstOrDefaultAsync(l => l.Id == id);

            if (loan == null)
            {
                return NotFound(new { message = "Loan not found." });
            }

            if (details)
            {
                DetailsLoanDto detailsLoanDto = _mapper.Map<DetailsLoanDto>(loan);
                return Ok(detailsLoanDto);
            }
            else
            {
                ShortLoanDto shortLoanDto = _mapper.Map<ShortLoanDto>(loan);
                return Ok(shortLoanDto);
            }
        }


        [HttpGet("all")]
        public async Task<IActionResult> GetAll([FromQuery] bool details = false,
                                                [FromQuery] bool onlyNotReturned = false,
                                                [FromQuery] bool onlyReturned = false,
                                                [FromQuery] int userId = -1)
        {
            var loans = await _context.Loans
                    .Include(l => l.User)
                    .Include(l => l.Book)
                    .Where(l => !onlyNotReturned || l.ReturnDate == null)
                    .Where(l => !onlyReturned || l.ReturnDate != null)
                    .Where(l => userId == -1 || l.UserId == userId)
                    .ToListAsync();

            if (details)
            {
                List<DetailsLoanDto> detailsLoanDto = _mapper.Map<List<DetailsLoanDto>>(loans);
                return Ok(detailsLoanDto);
            }
            else
            {
                List<ShortLoanDto> shortLoanDto = _mapper.Map<List<ShortLoanDto>>(loans);
                return Ok(shortLoanDto);
            }
        }

        [HttpPut("return/{id}")]
        public async Task<IActionResult> ReturnLoan([FromRoute] int id)
        {
            var loan = await _context.Loans.FirstOrDefaultAsync(l => l.Id == id);
            if (loan == null)
            {
                return NotFound(new { message = "Loan not found." });
            }

            if (loan.ReturnDate != null)
            {
                return BadRequest(new { message = "Loan has already been returned." });
            }

            loan.ReturnDate = DateOnly.FromDateTime(DateTime.Now);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Loan returned successfully." });
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateLoan([FromBody] CreateLoanDto createLoanDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == createLoanDto.UserId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == createLoanDto.BookId);
            if (book == null)
            {
                return NotFound(new { message = "Book not found." });
            }

            if (await _bookService.GetRelevantAvailableCopiesAsync(book.Id) <= 0)
            {
                return Conflict(new { message = "No available copies for the specified book." });
            }

            Loan loan = new Loan
            {
                UserId = user.Id,
                BookId = book.Id,
                LoanDate = DateOnly.FromDateTime(DateTime.Now),
                ReturnDate = null
            };

            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Loan created successfully.", LoanId = loan.Id });
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteLoan([FromBody] int id)
        {
            var loan = await _context.Loans.FirstOrDefaultAsync(l => l.Id == id);
            if (loan == null)
            {
                return NotFound(new { message = "Loan not found." });
            }
            if (loan.ReturnDate == null)
            {
                return BadRequest(new { message = "Cannot delete an active loan. Please return the book first." });
            }

            _context.Loans.Remove(loan);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Loan deleted successfully." });
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateLoan([FromRoute] int id, [FromBody] UpdateLoanDto updateLoanDto)
        {
            var loan = await _context.Loans.FirstOrDefaultAsync(l => l.Id == id);
            if (loan == null)
            {
                return NotFound(new { message = "Loan not found." });
            }

            User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == updateLoanDto.UserId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            Book? book = await _context.Books.FirstOrDefaultAsync(b => b.Id == updateLoanDto.BookId);
            if (book == null)
            {
                return NotFound(new { message = "Book not found." });
            }

            if (await _bookService.GetRelevantAvailableCopiesAsync(book.Id) <= 0)
            {
                return Conflict(new { message = "No available copies for the specified book." });
            }

            _mapper.Map(updateLoanDto, loan);

            _context.Update(loan);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Loan updated successfully." });
        }
    }
}
