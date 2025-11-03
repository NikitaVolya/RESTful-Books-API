using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RESTful_Books_API.Data;
using RESTful_Books_API.DTO.Book;
using RESTful_Books_API.Services;

namespace RESTful_Books_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly BookService _bookService;

        public BooksController(AppDbContext context, IMapper mapper, BookService bookService)
        {
            _context = context;
            _mapper = mapper;
            _bookService = bookService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, [FromQuery] bool details = false)
        {
            var book = await _context.Books
                .Include(b => b.Loans)
                .ThenInclude(l => l.User)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
            {
                return NotFound(new { message = "Book not found." });
            }

            if (details)
            {
                DetailsBookDto bookDto = _mapper.Map<DetailsBookDto>(book);
                return Ok(bookDto);
            }
            else
            {
                ShortBookDto bookDto = _mapper.Map<ShortBookDto>(book);
                return Ok(bookDto);
            }
        }

        [HttpGet("isbn/{isbn}")]
        public async Task<IActionResult> GetByIsbn(int isbn, [FromQuery] bool details = false)
        {
            var book = await _context.Books
                .Include(b => b.Loans)
                .ThenInclude(l => l.User)
                .FirstOrDefaultAsync(b => b.ISBN == isbn);

            if (book == null)
            {
                return NotFound(new { message = "Book not found." });
            }

            if (details)
            {
                DetailsBookDto bookDto = _mapper.Map<DetailsBookDto>(book);
                return Ok(bookDto);
            }
            else
            {
                ShortBookDto bookDto = _mapper.Map<ShortBookDto>(book);
                return Ok(bookDto);
            }
        }


        [HttpGet("all")]
        public async Task<IActionResult> GetAll([FromQuery] bool details = false, 
                                                [FromQuery] string? author = null)
        {
            var books = await _context.Books
                .Include(b => b.Loans)
                .ThenInclude(l => l.User)
                .Where(b => string.IsNullOrEmpty(author) || b.Author.ToLower().Contains(author.ToLower()))
                .ToListAsync();

            if (details)
            {
                List<DetailsBookDto> booksDto = _mapper.Map<List<DetailsBookDto>>(books);

                return Ok(booksDto);
            }
            else
            {
                List<ShortBookDto> booksDto = _mapper.Map<List<ShortBookDto>>(books);

                return Ok(booksDto);
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] ShortBookDto bookDto)
        {
            if (!await _bookService.BookTitleIsUniqueAsync(bookDto.Title))
            {
                return BadRequest(new { message = "A book with the same title already exists." });
            }

            var book = _mapper.Map<Models.Book>(bookDto);

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            var createdBookDto = _mapper.Map<ShortBookDto>(book);

            return CreatedAtAction(nameof(GetById), new { id = book.Id }, createdBookDto);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound(new { message = "Book not found." });
            }

            if (book.CopiesAvailable != await _bookService.GetRelevantAvailableCopiesAsync(id))
            {
                return BadRequest(new { message = "Cannot delete a book that has active loans." });
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ShortBookDto bookDto)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound(new { message = "Book not found." });
            }
            if (book.Title != bookDto.Title && !await _bookService.BookTitleIsUniqueAsync(bookDto.Title))
            {
                return BadRequest(new { message = "A book with the same title already exists." });
            }

            _mapper.Map(bookDto, book);
            _context.Books.Update(book);
            await _context.SaveChangesAsync();

            var updatedBookDto = _mapper.Map<ShortBookDto>(book);
            return Ok(updatedBookDto);
        }
    }
}
