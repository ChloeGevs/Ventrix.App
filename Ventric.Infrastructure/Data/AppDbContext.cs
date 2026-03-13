using Microsoft.EntityFrameworkCore;
using Ventrix.Domain.Models;

namespace Ventrix.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<BorrowRecord> BorrowRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) // Changed from ModelCreatingBuilder to ModelBuilder
        {
            base.OnModelCreating(modelBuilder);

            // Ensure UserId is treated as the Primary Key
            modelBuilder.Entity<User>().HasKey(u => u.UserId);

            // Set up relationships to prevent accidental deletion of items with history
            modelBuilder.Entity<BorrowRecord>()
                .HasOne(b => b.InventoryItem) // Correctly mapping the navigation property
                .WithMany()
                .HasForeignKey(b => b.InventoryItemId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}