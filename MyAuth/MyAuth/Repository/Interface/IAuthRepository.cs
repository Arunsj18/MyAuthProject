using MyAuth.Models;

namespace MyAuth.Repositories
{
    public interface IAuthRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<int> AddUserAsync(User user);
    }
}