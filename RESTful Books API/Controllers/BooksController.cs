using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RESTful_Books_API.Data;
using RESTful_Books_API.DTO.Book;

namespace RESTful_Books_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public BooksController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var books = await _context.Books.ToListAsync();

            List<ShortBookDto> booksDto = _mapper.Map<List<ShortBookDto>>(books);

            return Ok(booksDto);
        }
    }
}
