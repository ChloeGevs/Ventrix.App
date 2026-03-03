using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ventrix.Domain.Models;
using Ventrix.Infrastructure.Data;

namespace Ventrix.Application.Services
{
    public class InventoryService
    {
        private readonly AppDbContext _context;

        public InventoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddItemAsync(string name, string category, string status, string condition)
        {
            var newItem = new InventoryItem
            {
                Name = name,
                Category = (ItemCategory)Enum.Parse(typeof(ItemCategory), category),
                Status = (ItemStatus)Enum.Parse(typeof(ItemStatus), status),
                Condition = condition,
                DateAdded = DateTime.Now
            };

            _context.InventoryItems.Add(newItem);
            await _context.SaveChangesAsync();
        }

        public async Task<List<InventoryItem>> GetAllItemsAsync()
        {
            return await _context.InventoryItems.ToListAsync();
        }

        public async Task<List<InventoryItem>> GetFilteredInventoryAsync(string statusFilter = "All", string searchTerm = "")
        {
            var query = _context.InventoryItems.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(i => i.Name.ToLower().Contains(searchTerm));
            }

            if (statusFilter != "All" && Enum.TryParse(statusFilter, out ItemStatus filterEnum))
            {
                query = query.Where(i => i.Status == filterEnum);
            }

            return await query.ToListAsync();
        }

        public async Task<InventoryItem> GetItemByIdAsync(int id)
        {
            return await _context.InventoryItems.FindAsync(id);
        }

        public async Task UpdateItemAsync(int id, string name, string category, string status, string condition)
        {
            var item = await _context.InventoryItems.FindAsync(id);
            if (item != null)
            {
                item.Name = name;
                item.Category = (ItemCategory)Enum.Parse(typeof(ItemCategory), category);
                item.Status = (ItemStatus)Enum.Parse(typeof(ItemStatus), status);
                item.Condition = condition;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteItemAsync(int id)
        {
            var item = await _context.InventoryItems.FindAsync(id);
            if (item != null)
            {
                _context.InventoryItems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }
    }
}