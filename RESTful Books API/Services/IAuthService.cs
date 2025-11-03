using RESTful_Books_API.Models;


namespace RESTful_Books_API.Services
{
    public interface IAuthService
    {
        Task<User?> ValidateUserAsync(string username, string password);
        string HashPassword(string password);
    }
}
