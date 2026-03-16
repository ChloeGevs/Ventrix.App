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
            var hashedPassword = HashPassword(dto.Password);
            return await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == dto.UserId && u.Password == hashedPassword);
        }

        public async Task<User> RegisterNewBorrowerAsync(RegisterDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FirstName) || string.IsNullOrWhiteSpace(dto.LastName))
                throw new ArgumentException("Names are required for registration.");

            if (string.IsNullOrWhiteSpace(dto.UserId))
                throw new ArgumentException("An ID number is required.");

            // Ensure the exact ID isn't already taken
            if (await _context.Users.AnyAsync(u => u.UserId == dto.UserId))
                throw new ArgumentException("This exact ID is already registered in the system.");

            var newUser = new User
            {
                UserId = dto.UserId, // Saves the exact ID typed by the Admin!
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Role = dto.Role,
                CreatedAt = DateTime.Now,
                Strikes = 0
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            return newUser;
        }
        public async Task DeleteUserAsync(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                // SAFETY CHECK: Do not allow deletion if they are holding items!
                var hasActiveItems = await _context.BorrowRecords.AnyAsync(b =>
                    b.BorrowerId == userId && (b.Status == BorrowStatus.Active || b.Status == BorrowStatus.Overdue));

                if (hasActiveItems)
                {
                    throw new Exception("Cannot delete this account. The user is currently holding active or overdue items. They must return them first.");
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
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