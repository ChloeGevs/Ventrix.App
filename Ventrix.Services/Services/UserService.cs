using Ventrix.Domain.Interfaces;
using User = Ventrix.Domain.Models.User;

namespace Ventrix.Application.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepo;

        public UserService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public void InitializeDefaultAdmin()
        {
            _userRepo.SeedAdminUser();
        }

        public User Login(string userId, string password)
        {
            return _userRepo.GetByCredentials(userId, password);
        }

        public void RegisterNewBorrower(User user)
        {
            if (string.IsNullOrWhiteSpace(user.FirstName) || string.IsNullOrWhiteSpace(user.LastName))
                throw new ArgumentException("Names are required for registration.");

            _userRepo.Add(user);
        }
    }
}