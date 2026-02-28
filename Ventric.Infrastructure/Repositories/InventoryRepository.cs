using Ventrix.Domain.Interfaces;
using Ventrix.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Ventrix.Infrastructure.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly AppDbContext _context;
        public InventoryRepository(AppDbContext context) => _context = context;

        // Optimized for reading
        public IEnumerable<InventoryItem> GetAll() =>
            _context.InventoryItems.AsNoTracking().ToList();

        public InventoryItem GetById(int id) => _context.InventoryItems.Find(id);

        public void Add(InventoryItem item)
        {
            _context.InventoryItems.Add(item);
            _context.SaveChanges();
        }

        public void Update(InventoryItem item)
        {
            _context.InventoryItems.Update(item);
            _context.SaveChanges();
        }

        // Consolidated removal logic
        public void Remove(InventoryItem item)
        {
            _context.InventoryItems.Remove(item);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var item = GetById(id);
            if (item != null) Remove(item);
        }

        // Count methods for Dashboard Stats
        public int GetTotalCount() => _context.InventoryItems.Count();

        public int GetCountByStatus(string status) =>
            _context.InventoryItems.Count(i => i.Status == status);

        public int GetCountByCondition(string condition) =>
            _context.InventoryItems.Count(i => i.Condition == condition);
    }
}