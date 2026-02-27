using Ventrix.Domain.Interfaces;
using Ventrix.Domain.Models;

namespace Ventrix.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context) => _context = context;

        public User GetByCredentials(string userId, string password) =>
            _context.Users.FirstOrDefault(u => u.UserId == userId && u.Password == password);

        public void Add(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public bool Exists(string userId) => _context.Users.Any(u => u.UserId == userId);

        public void SeedAdminUser()
        {
            // Check if any admin already exists to avoid duplicates
            if (!_context.Users.Any(u => u.Role == "Admin"))
            {
                var admin = new User
                {
                    UserId = "admin",        // Your default username
                    FirstName = "System",
                    MiddleName = " ",
                    LastName = "Administrator",
                    Suffix = string.Empty,
                    Role = "Admin",
                    Password = "admin123",   // Your default password
                    CreatedAt = DateTime.Now
                };

                _context.Users.Add(admin);
                _context.SaveChanges();
            }
        }
    }
}