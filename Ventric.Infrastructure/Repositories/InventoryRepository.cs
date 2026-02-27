using Ventrix.Domain.Interfaces;
using Ventrix.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Ventrix.Infrastructure.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly AppDbContext _context;
        public InventoryRepository(AppDbContext context) => _context = context;

        public IEnumerable<InventoryItem> GetAll() => _context.InventoryItems.ToList();
        public InventoryItem GetById(int id) => _context.InventoryItems.Find(id);
        public void Add(InventoryItem item) { _context.InventoryItems.Add(item); _context.SaveChanges(); }
        public void Update(InventoryItem item) { _context.Entry(item).State = EntityState.Modified; _context.SaveChanges(); }
        public void Delete(int id) { var item = GetById(id); if (item != null) { _context.InventoryItems.Remove(item); _context.SaveChanges(); } }
        public int GetTotalCount() => _context.InventoryItems.Count();
        public int GetCountByStatus(string status) => _context.InventoryItems.Count(i => i.Status == status);
        public int GetCountByCondition(string condition) => _context.InventoryItems.Count(i => i.Condition == condition);
    }
}