using Ventrix.Domain.Models;

namespace Ventrix.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByCredentialsAsync(string userId, string password);
        Task AddAsync(User user);
        Task<bool> ExistsAsync(string userId);
        Task SeedAdminUserAsync();
    }

}