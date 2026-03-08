using Microsoft.EntityFrameworkCore;
using Ventrix.Domain.Models;

namespace Ventrix.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        // THE FIX: This allows your original Factory to pass in the connection string
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<BorrowRecord> BorrowRecords { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=ventrix.db");
            }
        }
    }
}