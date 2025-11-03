using Microsoft.EntityFrameworkCore;
using RESTful_Books_API.Data;
using RESTful_Books_API.Models;
using System.Security.Cryptography;
using System.Text;

namespace RESTful_Books_API.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;

        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UserModel?> ValidateUserAsync(string email, string password)
        {
            var hash = HashPassword(password);
            return await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == email && u.PasswordHash == hash);
        }

        public string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
