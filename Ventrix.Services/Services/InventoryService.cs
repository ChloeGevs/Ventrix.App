using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; // REQUIRED FOR TASK
using Microsoft.EntityFrameworkCore;
using Ventrix.Infrastructure;
using Ventrix.Domain.Models;

namespace Ventrix.Application.Services
{
    public class InventoryService
    {
        // Notice: 'Task' instead of 'void'
        public async Task AddItemAsync(string name, string category, string status, string condition)
        {
            using (var db = new AppDbContext())
            {
                var newItem = new InventoryItem
                {
                    Name = name,
                    Category = (ItemCategory)Enum.Parse(typeof(ItemCategory), category),
                    Status = (ItemStatus)Enum.Parse(typeof(ItemStatus), status),
                    Condition = condition,
                    DateAdded = DateTime.Now
                };
                db.InventoryItems.Add(newItem);
                await db.SaveChangesAsync();
            }
        }

        // Notice: 'Task<List<...>>' instead of just 'List<...>'
        public async Task<List<InventoryItem>> GetAllItemsAsync()
        {
            using (var db = new AppDbContext())
            {
                return await db.InventoryItems.ToListAsync();
            }
        }

        public async Task<List<InventoryItem>> GetFilteredInventoryAsync(string statusFilter = "All", string searchTerm = "")
        {
            using (var db = new AppDbContext())
            {
                var query = db.InventoryItems.AsQueryable();

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
        }

        public async Task<InventoryItem> GetItemByIdAsync(int id)
        {
            using (var db = new AppDbContext())
            {
                return await db.InventoryItems.FindAsync(id);
            }
        }

        public async Task UpdateItemAsync(int id, string name, string category, string status, string condition)
        {
            using (var db = new AppDbContext())
            {
                var item = await db.InventoryItems.FindAsync(id);
                if (item != null)
                {
                    item.Name = name;
                    item.Category = (ItemCategory)Enum.Parse(typeof(ItemCategory), category);
                    item.Status = (ItemStatus)Enum.Parse(typeof(ItemStatus), status);
                    item.Condition = condition;
                    await db.SaveChangesAsync();
                }
            }
        }

        // Notice: 'Task' instead of 'void' (Fixes CS4008 Cannot await void)
        public async Task DeleteItemAsync(int id)
        {
            using (var db = new AppDbContext())
            {
                var item = await db.InventoryItems.FindAsync(id);
                if (item != null)
                {
                    db.InventoryItems.Remove(item);
                    await db.SaveChangesAsync();
                }
            }
        }
    }
}