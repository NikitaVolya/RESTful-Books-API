using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RESTful_Books_API.Models;
using RESTful_Books_API.Data;
using AutoMapper;
using RESTful_Books_API.Services;
using RESTful_Books_API.DTO.User;

namespace RESTful_Books_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserService _userService;

        public UsersController(AppDbContext context, IMapper mapper, UserService userService)
        {
            _context = context;
            _mapper = mapper;
            _userService = userService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id, [FromQuery] bool details = false)
        {
            var user = await _context.Users.Include(u => u.Loans).ThenInclude(l => l.Book)
                                     .FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }
            if (details)
            {
                DetailsUserDto userDto = _mapper.Map<DetailsUserDto>(user);
                return Ok(userDto);
            } else {
                ShortUserDto userDto = _mapper.Map<ShortUserDto>(user);
                return Ok(userDto);
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers([FromQuery] bool details = false)
        {
            var users = _context.Users.Include(u => u.Loans).ThenInclude(l => l.Book);
           
            if (details)
            {
                List<DetailsUserDto> usersDto = _mapper.Map<List<DetailsUserDto>>(await users.ToListAsync());
                return Ok(usersDto);
            } else {
                List<ShortUserDto> usersDto = _mapper.Map<List<ShortUserDto>>(await users.ToListAsync());
                return Ok(usersDto);
            }

        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody] ShortUserDto createUserDto)
        {
            if (!await _userService.IsEmailUnique(createUserDto.Email))
            {
                return Conflict(new { message = "Email already in use." });
            }

            User user = _mapper.Map<User>(createUserDto);

            user.MembershipDate = DateOnly.FromDateTime(DateTime.Now);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAllUsers), new { id = user.Id }, user);
        }
    }
}
