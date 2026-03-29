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
            return await _context.InventoryItems.ToListAsync();
        }
        public async Task<List<InventoryItem>> GetTrueAvailableItemsAsync()
        {
            // 1. Get all items that are physically marked as 'Available'
            var physicallyAvailable = (await GetAllItemsAsync())
                .Where(i => i.Status == ItemStatus.Available)
                .ToList();

            // 2. Get all IDs of items currently sitting in 'Pending' borrow requests
            var pendingItemIds = await _context.BorrowRecords
                .Where(b => b.Status == BorrowStatus.Pending)
                .Select(b => b.InventoryItemId)
                .ToListAsync(); // Added Async here for better performance

            // 3. Return only items that are NOT in a pending request
            return physicallyAvailable
                .Where(i => !pendingItemIds.Contains(i.Id))
                .ToList();
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
            // Only seed if the table is completely empty
            if (await _context.InventoryItems.AnyAsync()) return;

            var seedItems = new List<InventoryItem>();

            // Helper to add multiple units of the same item name
            void PrepareItems(string name, int quantity, ItemCategory category)
            {
                for (int i = 1; i <= quantity; i++)
                {
                    seedItems.Add(new InventoryItem
                    {
                        // Formats as "Laptop #01", "Laptop #02", etc.
                        Name = $"{name} #{i:D2}",
                        Category = category,
                        Status = ItemStatus.Available,
                        Condition = Condition.Good,
                        DateAdded = DateTime.Now
                    });
                }
            }

            // Your specific previous items
            PrepareItems("Laptop", 40, ItemCategory.Electronics);
            PrepareItems("Tablets", 50, ItemCategory.Electronics);
            PrepareItems("Projector", 2, ItemCategory.Electronics);
            PrepareItems("Headphones", 30, ItemCategory.Peripherals);
            PrepareItems("Hdmi", 3, ItemCategory.Peripherals);
            PrepareItems("Speaker", 2, ItemCategory.Electronics);
            PrepareItems("Mouse", 40, ItemCategory.Peripherals);
            PrepareItems("Keyboard", 20, ItemCategory.Peripherals);
            PrepareItems("Chess board", 8, ItemCategory.Others);
            PrepareItems("Sudoku board", 4, ItemCategory.Others);
            PrepareItems("Word factory", 2, ItemCategory.Others);
            PrepareItems("Games of general board", 1, ItemCategory.Others);
            PrepareItems("Meter stick", 30, ItemCategory.Others);
            PrepareItems("Flash drive", 5, ItemCategory.Peripherals);
            PrepareItems("Laptop bags", 40, ItemCategory.Others);
            PrepareItems("Chairs/mono blocks", 40, ItemCategory.Others);

            await _context.InventoryItems.AddRangeAsync(seedItems);
            await _context.SaveChangesAsync();
        }
    }
}