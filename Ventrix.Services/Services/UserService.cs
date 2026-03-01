using System;
using System.Threading.Tasks; // Required for Task
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

        public async Task InitializeDefaultAdminAsync()
        {
            await _userRepo.SeedAdminUserAsync(); // Updated to async
        }

        public async Task<User> LoginAsync(string userId, string password)
        {
            return await _userRepo.GetByCredentialsAsync(userId, password); // Updated to async
        }

        public async Task RegisterNewBorrowerAsync(User user)
        {
            if (string.IsNullOrWhiteSpace(user.FirstName) || string.IsNullOrWhiteSpace(user.LastName))
                throw new ArgumentException("Names are required for registration.");

            await _userRepo.AddAsync(user); // Updated to async
        }
    }
}