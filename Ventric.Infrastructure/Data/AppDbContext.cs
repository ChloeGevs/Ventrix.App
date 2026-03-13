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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Primary Key Configuration
            modelBuilder.Entity<User>().HasKey(u => u.UserId);

            // 2. Inventory Item Configuration
            // Ensure ID is the primary key for items
            modelBuilder.Entity<InventoryItem>().HasKey(i => i.Id);

            // 3. BorrowRecord Relationships
            modelBuilder.Entity<BorrowRecord>(entity =>
            {
                entity.HasKey(b => b.Id);

                // Mapping the Borrower (User)
                entity.HasOne(b => b.Borrower)
                    .WithMany()
                    .HasForeignKey(b => b.BorrowerId)
                    .OnDelete(DeleteBehavior.Restrict); // Prevent deleting users with active history

                // Mapping the Inventory Item
                entity.HasOne(b => b.InventoryItem)
                    .WithMany()
                    .HasForeignKey(b => b.InventoryItemId)
                    .OnDelete(DeleteBehavior.Restrict); // Critical for soft-delete logic
            });
        }
    }
}