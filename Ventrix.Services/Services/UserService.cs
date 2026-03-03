using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ventrix.Domain.Models;
using Ventrix.Application.DTOs;
using Ventrix.Infrastructure.Data;

namespace Ventrix.Application.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task InitializeDefaultAdminAsync()
        {
            var adminExists = await _context.Users.AnyAsync(u => u.Role == UserRole.Admin);
            if (!adminExists)
            {
                var admin = new User
                {
                    UserId = "admin",
                    Password = "password", // Remember to hash in a real app!
                    FirstName = "System",
                    LastName = "Admin",
                    Role = UserRole.Admin
                };
                _context.Users.Add(admin);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<User> LoginAsync(LoginDto dto)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == dto.UserId && u.Password == dto.Password);
        }

        // Notice we changed Task to Task<User>
        public async Task<User> RegisterNewBorrowerAsync(RegisterDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FirstName) || string.IsNullOrWhiteSpace(dto.LastName))
                throw new ArgumentException("Names are required for registration.");

            var newUser = new User
            {
                UserId = new Random().Next(20240000, 20249999).ToString(),
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Suffix = dto.Suffix,
                Role = Enum.TryParse<UserRole>(dto.Role, out var parsedRole) ? parsedRole : UserRole.Student,
                Password = dto.Password, // Make sure your DTO is passing this!
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return newUser; // Return the user so the form can see the ID!
        }
    }
}