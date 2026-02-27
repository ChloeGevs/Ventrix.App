using Ventrix.Domain.Models;

namespace Ventrix.Domain.Interfaces
{
    public interface IUserRepository
    {
        User GetByCredentials(string userId, string password);
        void Add(User user);
        bool Exists(string userId);

        void SeedAdminUser();
    }

}