using RESTful_Books_API.Models;


namespace RESTful_Books_API.Services
{
    public interface IAuthService
    {
        Task<UserModel?> ValidateUserAsync(string username, string password);

        Task<AdminModel?> ValidateAdminAsync(string username, string password);

        string HashPassword(string password);
    }
}
