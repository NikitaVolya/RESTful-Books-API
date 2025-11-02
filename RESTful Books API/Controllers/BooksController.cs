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


        [HttpGet("all")]
        public async Task<IActionResult> GetAll([FromQuery] bool details = false)
        {
            var books = await _context.Books
                .Include(b => b.Loans)
                .ThenInclude(l => l.User)
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
    }
}
