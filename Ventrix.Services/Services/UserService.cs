// File: chloegevs/ventrix.app/Ventrix.App-proponent-1/Ventrix.Services/Services/UserService.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
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

        // FIX: Added the missing InitializeDefaultAdminAsync method
        public async Task InitializeDefaultAdminAsync()
        {
            var adminExists = await _context.Users.AnyAsync(u => u.Role == UserRole.Admin || u.UserId == "admin");
            if (!adminExists)
            {
                var admin = new User
                {
                    UserId = "admin",
                    Password = HashPassword("admin123"), // Hashes default password
                    FirstName = "System",
                    LastName = "Admin",
                    Role = UserRole.Admin,
                    CreatedAt = DateTime.Now
                };
                _context.Users.Add(admin);
                await _context.SaveChangesAsync();
            }
        }

        private string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password)) return null;
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        public async Task<User> LoginAsync(LoginDTO dto)
        {
            // Security Fix: Always check hashed password rather than hardcoded bypass
            var hashedPassword = HashPassword(dto.Password);
            return await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == dto.UserId && u.Password == hashedPassword);
        }

        public async Task<User> RegisterNewBorrowerAsync(RegisterDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FirstName) || string.IsNullOrWhiteSpace(dto.LastName))
                throw new ArgumentException("Names are required for registration.");

            string rolePrefix = dto.Role == UserRole.Faculty ? "F" : "S";
            string currentYear = DateTime.Now.Year.ToString();

            // Logic Fix: Better ID generation to prevent collisions
            var lastUser = await _context.Users
                .Where(u => u.UserId.StartsWith($"{currentYear}-{rolePrefix}"))
                .OrderByDescending(u => u.UserId)
                .FirstOrDefaultAsync();

            int nextNum = 1;
            if (lastUser != null && lastUser.UserId.Contains("-"))
            {
                var parts = lastUser.UserId.Split('-');
                if (parts.Length == 3 && int.TryParse(parts[2], out int lastNum))
                {
                    nextNum = lastNum + 1;
                }
            }

            string generatedUserId = $"{currentYear}-{rolePrefix}-{nextNum:D4}";

            var newUser = new User
            {
                UserId = generatedUserId,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                MiddleName = dto.MiddleName,
                Suffix = dto.Suffix,
                Role = dto.Role,
                Password = HashPassword("1234"), // Default password hashed
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            return newUser;
        }

        public async Task AddStrikeAsync(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.Strikes++;
                await _context.SaveChangesAsync();
            }
        }

        public async Task ClearStrikesAsync(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.Strikes = 0;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }
    }
}