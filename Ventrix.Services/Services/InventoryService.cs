using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ventrix.Domain.Models;
using Ventrix.Infrastructure.Data;
using Ventrix.Domain.Enums;

namespace Ventrix.Application.Services
{
    public class InventoryService
    {
        private readonly AppDbContext _context;

        public InventoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddItemAsync(string name, string category, string status, Condition condition)
        {
            // FIX: Case-insensitive parsing prevents crashes from UI string variations
            if (!Enum.TryParse(category, true, out ItemCategory catEnum)) catEnum = ItemCategory.Others;
            if (!Enum.TryParse(status, true, out ItemStatus statEnum)) statEnum = ItemStatus.Available;

            var newItem = new InventoryItem
            {
                Name = name,
                Category = catEnum,
                Status = statEnum,
                Condition = condition,
                DateAdded = DateTime.Now
            };

            _context.InventoryItems.Add(newItem);
            await _context.SaveChangesAsync();
        }

        public async Task<List<InventoryItem>> GetAllItemsAsync()
        {
            // Fix: Only return items that are not "Archived" (Soft Delete)
            return await _context.InventoryItems
                .Where(i => i.Status != ItemStatus.Unavailable || i.Condition != Condition.Damaged)
                .ToListAsync();
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

        public async Task UpdateItemAsync(int id, string name, string category, string status, Condition condition)
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
                // check if the item has history before actual deletion
                var hasHistory = await _context.BorrowRecords.AnyAsync(b => b.InventoryItemId == id);

                if (hasHistory)
                {
                    // Soft delete logic: mark as unavailable/archived instead of removing
                    item.Status = ItemStatus.Unavailable;
                }
                else
                {
                    _context.InventoryItems.Remove(item);
                }

                await _context.SaveChangesAsync();
            }
        }

        public async Task SeedInventoryAsync(List<InventoryItem> items)
        {
            foreach (var item in items)
            {
                item.DateAdded = DateTime.Now;
                _context.InventoryItems.Add(item);
            }
            await _context.SaveChangesAsync();
        }

        public async Task RunInitialSeed()
        {
            if (await _context.InventoryItems.AnyAsync()) return;

            var seedItems = new List<InventoryItem>
    {
        new InventoryItem { Name = "Projector A", Category = ItemCategory.Electronics, Status = ItemStatus.Available, Condition = Condition.Good, DateAdded = DateTime.Now },
        new InventoryItem { Name = "HDMI Cable 5m", Category = ItemCategory.Accessories, Status = ItemStatus.Available, Condition = Condition.Good, DateAdded = DateTime.Now }
    };

            _context.InventoryItems.AddRange(seedItems);
            await _context.SaveChangesAsync();
        }
    }
}