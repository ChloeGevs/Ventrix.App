using Microsoft.EntityFrameworkCore;
using Ventrix.Domain.Models;

namespace Ventrix.Infrastructure
{
    public class AppDbContext : DbContext
    {
        // Constructor that accepts DbContextOptions, allowing the UI layer 
        // to inject the connection string at runtime.
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<BorrowRecord> BorrowRecords { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Leave this empty or remove it. Configuration now happens 
            // in the Program.cs of your Ventrix.App project.
        }
    }
}