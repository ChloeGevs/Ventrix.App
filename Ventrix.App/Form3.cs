using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Ventrix.Infrastructure;
using Ventrix.Domain.Models;

namespace Ventrix.App.Services
{
    public class InventoryService
    {
        // CREATE: Add a new item to the database
        public void CreateItem(string name, string category, string status, string condition)
        {
            using (var db = new AppDbContext())
            {
                var newItem = new InventoryItem
                {
                    Name = name,
                    Category = category,
                    Status = status,
                    Condition = condition,
                    DateAdded = DateTime.Now
                };
                db.InventoryItems.Add(newItem);
                db.SaveChanges();
            }
        }

        // READ: Fetch items based on a status filter or search term
        public List<InventoryItem> GetItems(string statusFilter = "All", string searchTerm = "")
        {
            using (var db = new AppDbContext())
            {
                var query = db.InventoryItems.AsQueryable();

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    searchTerm = searchTerm.ToLower();
                    query = query.Where(i => i.Name.ToLower().Contains(searchTerm) ||
                                             i.Category.ToLower().Contains(searchTerm));
                }

                if (statusFilter != "All")
                {
                    query = query.Where(i => i.Status == statusFilter);
                }

                return query.ToList();
            }
        }

        // UPDATE: Modify an existing record
        public void UpdateItem(int id, string name, string category, string status, string condition)
        {
            using (var db = new AppDbContext())
            {
                var item = db.InventoryItems.Find(id);
                if (item != null)
                {
                    item.Name = name;
                    item.Category = category;
                    item.Status = status;
                    item.Condition = condition;
                    db.SaveChanges();
                }
            }
        }

        // DELETE: Remove a record from SQLite
        public void DeleteItem(int id)
        {
            using (var db = new AppDbContext())
            {
                var item = db.InventoryItems.Find(id);
                if (item != null)
                {
                    db.InventoryItems.Remove(item);
                    db.SaveChanges();
                }
            }
        }

        // Dashboard Stats Logic
        public (int Total, int Available, int Borrowed) GetDashboardStats()
        {
            using (var db = new AppDbContext())
            {
                return (
                    db.InventoryItems.Count(),
                    db.InventoryItems.Count(x => x.Status == "Available"),
                    db.InventoryItems.Count(x => x.Status == "Borrowed")
                );
            }
        }
    }
}