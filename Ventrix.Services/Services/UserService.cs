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

        public async Task<User> GetUserByIdAsync(string userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        // --- NEW: Safely update a user, even if their Primary Key (School ID) changes ---
        public async Task UpdateUserWithIdChangeAsync(string oldUserId, User updatedData, string newUserId)
        {
            // If the ID didn't change, perform a standard update
            if (oldUserId == newUserId)
            {
                var existing = await _context.Users.FindAsync(oldUserId);
                if (existing != null)
                {
                    existing.FirstName = updatedData.FirstName;
                    existing.LastName = updatedData.LastName;
                    existing.Suffix = updatedData.Suffix;
                    existing.Role = updatedData.Role;
                    await _context.SaveChangesAsync();
                }
                return;
            }

            // If the ID DID change, ensure the new ID isn't already taken
            if (await _context.Users.AnyAsync(u => u.UserId == newUserId))
                throw new Exception("The new School ID is already in use by another account.");

            // Use a transaction to safely clone, migrate records, and delete the old account
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var oldUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == oldUserId);
                    if (oldUser == null) throw new Exception("Original user not found.");

                    // 1. Create the new user with the updated data
                    var newUser = new User
                    {
                        UserId = newUserId,
                        FirstName = updatedData.FirstName,
                        LastName = updatedData.LastName,
                        Suffix = updatedData.Suffix,
                        Role = updatedData.Role,
                        Password = oldUser.Password,     // Carry over existing data
                        CreatedAt = oldUser.CreatedAt,   // Carry over existing data
                        Strikes = oldUser.Strikes        // Carry over existing data
                    };

                    _context.Users.Add(newUser);
                    await _context.SaveChangesAsync();

                    // 2. Migrate their borrowing history to the new ID
                    var records = await _context.BorrowRecords.Where(b => b.BorrowerId == oldUserId).ToListAsync();
                    foreach (var record in records)
                    {
                        record.BorrowerId = newUserId;
                    }
                    await _context.SaveChangesAsync();

                    // 3. Delete the old user profile
                    var userToDelete = await _context.Users.FindAsync(oldUserId);
                    if (userToDelete != null)
                    {
                        _context.Users.Remove(userToDelete);
                        await _context.SaveChangesAsync();
                    }

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception("Failed to change School ID: " + ex.Message);
                }
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