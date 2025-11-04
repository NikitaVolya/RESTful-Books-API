using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RESTful_Books_API.Data;
using RESTful_Books_API.DTO.Admin;
using RESTful_Books_API.Models;
using RESTful_Books_API.Services;
using System.Data.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace RESTful_Books_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public AdminsController(AppDbContext context, IAuthService authService, IMapper mapper)
        {
            _context = context;
            _authService = authService;
            _mapper = mapper;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginAdminDto loginAdminDto)
        {
            AdminModel? adminModel = await _authService.ValidateAdminAsync(loginAdminDto.Username, loginAdminDto.Password);
            if (adminModel == null)
                return Unauthorized(new { message = "Invalid username or password." });

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("This_is_my_first_Test_Key_That_Is_Long_Enough_123!");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Role, "Admin")
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

        [Authorize(Roles="Admin")]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateAdminDto createAdminDto)
        {
            AdminModel? findAdmin = await _context.Admins.FirstOrDefaultAsync(a => a.Username == createAdminDto.Username);
            if (findAdmin != null)
                return BadRequest(new { message = "Username is already used " });

            AdminModel adminModel = _mapper.Map<AdminModel>(createAdminDto);

            adminModel.PasswordHash = _authService.HashPassword(createAdminDto.Password);

            await _context.Admins.AddAsync(adminModel);
            await _context.SaveChangesAsync();

            return Ok();
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            AdminModel? adminModel = await _context.Admins.FirstOrDefaultAsync(a => a.Id == id);
            if (adminModel == null)
            {
                return BadRequest(new { message = "Admin not found" });
            }

            _context.Admins.Remove(adminModel);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
