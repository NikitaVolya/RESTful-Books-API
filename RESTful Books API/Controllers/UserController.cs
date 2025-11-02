using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RESTful_Books_API.Models;
using RESTful_Books_API.Data;
using AutoMapper;
using RESTful_Books_API.Services;

namespace RESTful_Books_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserService _userService;

        public UserController(AppDbContext context, IMapper mapper, UserService userService)
        {
            _context = context;
            _mapper = mapper;
            _userService = userService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers([FromRoute] bool details = false)
        {
            var users = await _context.Users.Include(u => u.Loans)
                .ThenInclude(l => l.Book)
                .ToListAsync();

            if (details)
            {
                IList<DTO.DetailsUserDto> usersDto = _mapper.Map<IList<DTO.DetailsUserDto>>(users);
                return Ok(usersDto);
            } else {
                List<DTO.ShortUserDto> usersDto = _mapper.Map<List<DTO.ShortUserDto>>(users);
                return Ok(usersDto);
            }

        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody] DTO.ShortUserDto createUserDto)
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
