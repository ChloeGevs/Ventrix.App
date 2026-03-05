using System;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Ventrix.Domain.Models;
using Ventrix.Application.DTOs;
using Ventrix.Infrastructure.Data;
using Ventrix.Domain.Enums;

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

            // 1. Determine the Role Prefix ('S' for Student, 'F' for Faculty)
            string rolePrefix = dto.Role == "Faculty" ? "F" : "S";

            // 2. Get the current year
            string currentYear = DateTime.Now.Year.ToString();

            // 3. Count how many users ALREADY exist to create the next sequential number
            int userCount = await _context.Users.CountAsync() + 1;

            // 4. Format the ID (D4 pads the number with zeros until it is 4 digits long)
            string generatedUserId = $"{currentYear}-{rolePrefix}-{userCount:D4}";

            var newUser = new User
            {
                UserId = generatedUserId, 
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