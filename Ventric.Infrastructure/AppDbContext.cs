using Microsoft.EntityFrameworkCore;
using Ventrix.Domain;

namespace Ventrix.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<BorrowRecord> BorrowRecords { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=ventrix.db");
        }
    }
}
git commit -m "fixed conflicts from stash"
git config --global user.email "i.gulen.561080@umindanao.edu.ph"
git config --global user.name "Irish Gulen"