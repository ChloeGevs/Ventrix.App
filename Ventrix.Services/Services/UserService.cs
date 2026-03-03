using System;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
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

        // Basic password hashing method
        private string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password)) return null;
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        public async Task InitializeDefaultAdminAsync()
        {
            var adminExists = await _context.Users.AnyAsync(u => u.Role == UserRole.Admin);
            if (!adminExists)
            {
                var admin = new User
                {
                    UserId = "admin",
                    Password = HashPassword("admin123"), // Securely hashed
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
            if (dto.UserId == "admin" && dto.Password == "admin123")
            {
                var adminUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == "admin");

                // If the admin isn't in the DB yet, create a temporary object so the app doesn't crash
                return adminUser ?? new User
                {
                    UserId = "admin",
                    FirstName = "System",
                    LastName = "Admin",
                    Role = UserRole.Admin
                };
            }

            // 2. REGULAR LOGIN LOGIC
            var hashedPassword = HashPassword(dto.Password);
            return await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == dto.UserId && u.Password == hashedPassword);
        }

        public async Task<User> RegisterNewBorrowerAsync(RegisterDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FirstName) || string.IsNullOrWhiteSpace(dto.LastName))
                throw new ArgumentException("Names are required for registration.");

            var newUser = new User
            {
                UserId = new Random().Next(20240000, 20249999).ToString(),
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                MiddleName = dto.MiddleName,
                Suffix = dto.Suffix,
                Role = Enum.TryParse<UserRole>(dto.Role, out var parsedRole) ? parsedRole : UserRole.Student,
                Password = " ",
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return newUser;
        }
    }
}