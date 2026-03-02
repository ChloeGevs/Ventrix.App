using Ventrix.Domain.Interfaces;
using Ventrix.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace Ventrix.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context) => _context = context;

        public async Task<User> GetByCredentialsAsync(string userId, string password) =>
            await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId && u.Password == password);

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(string userId) =>
            await _context.Users.AnyAsync(u => u.UserId == userId);

        public async Task SeedAdminUserAsync()
        {
            await _context.Database.EnsureCreatedAsync();

            if (!await _context.Users.AnyAsync(u => u.Role == UserRole.Admin))
            {
                var admin = new User
                {
                    UserId = "admin",
                    FirstName = "System",
                    MiddleName = " ",
                    LastName = "Administrator",
                    Suffix = string.Empty,
                    Role = UserRole.Admin,
                    Password = "admin123",
                    CreatedAt = DateTime.Now
                };

                await _context.Users.AddAsync(admin);
                await _context.SaveChangesAsync();
            }
        }
    }
}