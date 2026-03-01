using Ventrix.Domain.Interfaces;
using Ventrix.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; // Required for Task

namespace Ventrix.Infrastructure.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly AppDbContext _context;
        public InventoryRepository(AppDbContext context) => _context = context;

        // Optimized for reading with async
        public async Task<IEnumerable<InventoryItem>> GetAllAsync() =>
            await _context.InventoryItems.AsNoTracking().ToListAsync();

        public async Task<InventoryItem> GetByIdAsync(int id) =>
            await _context.InventoryItems.FindAsync(id);

        public async Task AddAsync(InventoryItem item)
        {
            await _context.InventoryItems.AddAsync(item);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(InventoryItem item)
        {
            _context.InventoryItems.Update(item);
            await _context.SaveChangesAsync();
        }

        // Consolidated removal logic with async
        public async Task RemoveAsync(InventoryItem item)
        {
            _context.InventoryItems.Remove(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var item = await GetByIdAsync(id);
            if (item != null) await RemoveAsync(item);
        }

        public async Task<int> GetTotalCountAsync() =>
            await _context.InventoryItems.CountAsync();

        public async Task<int> GetCountByStatusAsync(string status) =>
            await _context.InventoryItems.CountAsync(i => i.Status == status);

        public async Task<int> GetCountByConditionAsync(string condition) =>
            await _context.InventoryItems.CountAsync(i => i.Condition == condition);
    }
}