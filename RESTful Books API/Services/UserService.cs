using Microsoft.EntityFrameworkCore;
using RESTful_Books_API.Data;

namespace RESTful_Books_API.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsEmailUnique(string email)
        {
            return !( await _context.Users.AnyAsync(u => u.Email == email));
        }
    }
}
