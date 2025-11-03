using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RESTful_Books_API.Data;
using RESTful_Books_API.DTO.Loan;
using RESTful_Books_API.DTO.User;
using RESTful_Books_API.Models;
using RESTful_Books_API.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RESTful_Books_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAuthService _authService;
        private readonly UserService _userService;

        public UsersController(AppDbContext context, IMapper mapper, IAuthService authService, UserService userService)
        {
            _context = context;
            _mapper = mapper;
            _authService = authService;
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
            }
            else
            {
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
            }
            else
            {
                List<ShortUserDto> usersDto = _mapper.Map<List<ShortUserDto>>(await users.ToListAsync());
                return Ok(usersDto);
            }

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto loginUserDto)
        {
            var user = await _authService.ValidateUserAsync(loginUserDto.Email, loginUserDto.Password);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("This_is_my_first_Test_Key_That_Is_Long_Enough_123!");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, "User")
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                    )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { Token = tokenString });
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            if (!await _userService.IsEmailUnique(createUserDto.Email))
            {
                return Conflict(new { message = "Email already in use." });
            }

            UserModel user = _mapper.Map<UserModel>(createUserDto);

            user.PasswordHash = _authService.HashPassword(createUserDto.Password);
            user.MembershipDate = DateOnly.FromDateTime(DateTime.Now);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User registered successfully.");
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateUser([FromRoute] int id, [FromBody] ShortUserDto updateUserDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            if (user.Email != updateUserDto.Email && !await _userService.IsEmailUnique(updateUserDto.Email))
            {
                return Conflict(new { message = "Email already in use." });
            }

            _mapper.Map(updateUserDto, user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User updated successfully." });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] int id)
        {
            var user = await _context.Users.Include(u => u.Loans).FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }
            if (user.Loans.Any(l => l.ReturnDate == null))
            {
                return BadRequest(new { message = "User has active loans and cannot be deleted." });
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User deleted successfully." });
        }

        [Authorize(Roles = "User")]
        [HttpGet("loans")]
        public async Task<IActionResult> GetLoansForCurrentUser([FromQuery] bool details = false,
                                                                [FromQuery] bool onlyNotReturned = false,
                                                                [FromQuery] bool onlyReturned = false)
        {
            string? userEmailClaim = User.FindFirstValue(ClaimTypes.Email);

            if (userEmailClaim == null)
            {
                return Unauthorized(new { message = "User email claim not found." });
            }
            UserModel? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmailClaim);

            var loans = await _context.Loans
                    .Include(l => l.User)
                    .Include(l => l.Book)
                    .Where(l => l.User.Email == userEmailClaim)
                    .Where(l => !onlyNotReturned || l.ReturnDate == null)
                    .Where(l => !onlyReturned || l.ReturnDate != null)
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

        [Authorize(Roles = "User")]
        [HttpPost("loan/book/{id}")]
        public async Task<IActionResult> LoanBookForCurrentUser([FromRoute] int id)
        {
            string? userEmailClaim = User.FindFirstValue(ClaimTypes.Email);
            if (userEmailClaim == null)
            {
                return Unauthorized(new { message = "User email claim not found." });
            }
            UserModel? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmailClaim);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == id);
            if (book == null)
            {
                return NotFound(new { message = "Book not found." });
            }
            var bookService = new BookService(_context);

            if (!await bookService.IsBookAvailableAsync(id))
            {
                return BadRequest(new { message = "No available copies for this book." });
            }

            LoanModel loan = new LoanModel
            {
                UserId = user.Id,
                BookId = book.Id,
                LoanDate = DateOnly.FromDateTime(DateTime.Now),
                ReturnDate = null
            };
            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Book loaned successfully." });
        }

        [HttpPut("loan/return/{id}")]
        public async Task<IActionResult> ReturnLoanById([FromRoute] int id)
        {
            string? userEmailClaim = User.FindFirstValue(ClaimTypes.Email);
            if (userEmailClaim == null)
            {
                return Unauthorized(new { message = "User email claim not found." });
            }
            UserModel? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmailClaim);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            var loan = await _context.Loans.FirstOrDefaultAsync(l => l.Id == id && l.UserId == user.Id);
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

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            string? userEmailClaim = User.FindFirstValue(ClaimTypes.Email);
            if (userEmailClaim == null)
            {
                return Unauthorized(new { message = "User email claim not found." });
            }
            UserModel? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmailClaim);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            if (changePasswordDto.NewPassword != changePasswordDto.CurrentPassword)
            {
                return BadRequest(new { message = "New password and current do not match." });
            }

            var currentPasswordHash = _authService.HashPassword(changePasswordDto.CurrentPassword);
            if (user.PasswordHash != currentPasswordHash)
            {
                return BadRequest(new { message = "Current password is incorrect." });
            }

            user.PasswordHash = _authService.HashPassword(changePasswordDto.NewPassword);

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Password changed successfully." });
        }
    }
}
