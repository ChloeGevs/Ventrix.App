using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ventrix.Infrastructure;
using Ventrix.Domain.Models;

namespace Ventrix.Application.Services
{
    public class BorrowService
    {
        public async Task<List<BorrowRecord>> GetAllBorrowRecordsAsync()
        {
            using (var db = new AppDbContext())
            {
                return await db.BorrowRecords
                    .Include(b => b.Borrower)
                    .Include(b => b.Item)
                    .ToListAsync();
            }
        }

        public async Task AddBorrowRecordAsync(int userId, int itemId, int quantity, string purpose, string gradeLevel)
        {
            using (var db = new AppDbContext())
            {
                var record = new BorrowRecord
                {
                    UserId = userId,
                    InventoryItemId = itemId,
                    Quantity = quantity,
                    Purpose = purpose,
                    GradeLevel = gradeLevel,
                    BorrowDate = DateTime.Now,
                    Status = BorrowStatus.Active
                };

                var item = await db.InventoryItems.FindAsync(itemId);
                if (item != null)
                {
                    item.Status = ItemStatus.Borrowed;
                }

                db.BorrowRecords.Add(record);
                await db.SaveChangesAsync();
            }
        }

        public async Task ReturnItemAsync(int borrowRecordId)
        {
            using (var db = new AppDbContext())
            {
                var record = await db.BorrowRecords.Include(b => b.Item).FirstOrDefaultAsync(b => b.Id == borrowRecordId);
                if (record != null)
                {
                    record.Status = BorrowStatus.Returned;
                    record.ReturnDate = DateTime.Now;

                    if (record.Item != null)
                    {
                        record.Item.Status = ItemStatus.Available;
                    }
                    await db.SaveChangesAsync();
                }
            }
        }

        public async Task ProcessBorrowAsync(BorrowRecord record, int itemId)
        {
            using (var db = new AppDbContext())
            {
                record.InventoryItemId = itemId;
                record.BorrowDate = DateTime.Now;

                var item = await db.InventoryItems.FindAsync(itemId);
                if (item != null)
                {
                    item.Status = ItemStatus.Borrowed;
                }

                db.BorrowRecords.Add(record);
                await db.SaveChangesAsync();
            }
        }
        public async Task ClearAllActivityAsync()
        {
            using (var db = new AppDbContext())
            {
                var allRecords = await db.BorrowRecords.ToListAsync();
                db.BorrowRecords.RemoveRange(allRecords);
                await db.SaveChangesAsync();
            }
        }
    }
}