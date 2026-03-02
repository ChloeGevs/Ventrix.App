using System;
using System.Threading.Tasks;
using Ventrix.Domain.Interfaces;
using Ventrix.Domain.Models; // Ensure we have the Enums
using Ventrix.Application.DTOs; // Your new DTO namespace

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
            await _userRepo.SeedAdminUserAsync();
        }

        // Updated to accept the LoginDto
        public async Task<User> LoginAsync(LoginDto dto)
        {
            // Pass the DTO's properties down to your existing repository method
            return await _userRepo.GetByCredentialsAsync(dto.UserId, dto.Password);
        }

        // Updated to accept the RegisterDto
        public async Task RegisterNewBorrowerAsync(RegisterDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FirstName) || string.IsNullOrWhiteSpace(dto.LastName))
                throw new ArgumentException("Names are required for registration.");

            // Map the lightweight DTO into your full Domain Model
            var newUser = new User
            {
                UserId = new Random().Next(20240000, 20249999).ToString(),
                FirstName = dto.FirstName,
                LastName = dto.LastName,

                // Parse the string from the DTO into the Enum securely
                Role = Enum.TryParse<UserRole>(dto.Role, out var parsedRole) ? parsedRole : UserRole.Student,

                Password = dto.Password,
                CreatedAt = DateTime.Now
            };

            // Send the fully formed entity to the repository
            await _userRepo.AddAsync(newUser);
        }
    }
}