using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ventrix.Domain.Models;
using Ventrix.Infrastructure.Data;

namespace Ventrix.Application.Services
{
    public class BorrowService
    {
        private readonly AppDbContext _context;

        // Inject the DbContext directly
        public BorrowService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<BorrowRecord>> GetAllBorrowRecordsAsync()
        {
            return await _context.BorrowRecords
                .Include(b => b.Borrower)
                .Include(b => b.Item)
                .ToListAsync();
        }

        public async Task AddBorrowRecordAsync(int userId, int itemId, int quantity, string purpose, string gradeLevel)
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

            var item = await _context.InventoryItems.FindAsync(itemId);
            if (item != null)
            {
                item.Status = ItemStatus.Borrowed;
            }

            _context.BorrowRecords.Add(record);
            await _context.SaveChangesAsync();
        }

        public async Task ReturnItemAsync(int borrowRecordId)
        {
            var record = await _context.BorrowRecords
                .Include(b => b.Item)
                .FirstOrDefaultAsync(b => b.Id == borrowRecordId);

            if (record != null)
            {
                record.Status = BorrowStatus.Returned;
                record.ReturnDate = DateTime.Now;

                if (record.Item != null)
                {
                    record.Item.Status = ItemStatus.Available;
                }

                await _context.SaveChangesAsync();
            }
        }

        public async Task ProcessBorrowAsync(BorrowRecord record, int itemId)
        {
            // 1. Find the user in the database to verify their true role
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == record.BorrowerId);

            if (user == null)
            {
                throw new Exception("Borrower ID not found in the system. Please register first.");
            }

            // 2. ENFORCE SPECIFIC LIMITS BASED ON ROLE
            if (user.Role == UserRole.Student && record.Quantity > 2)
            {
                throw new Exception($"Borrow limit exceeded. Students are only allowed to borrow up to 2 items at a time.");
            }
            else if (user.Role == UserRole.Faculty && record.Quantity > 10)
            {
                throw new Exception($"Borrow limit exceeded. Faculty members are only allowed to borrow up to 10 items at a time.");
            }

            record.InventoryItemId = itemId;
            record.BorrowDate = DateTime.Now;

            var item = await _context.InventoryItems.FindAsync(itemId);
            if (item != null)
            {
                item.Status = ItemStatus.Borrowed;
            }

            _context.BorrowRecords.Add(record);
            await _context.SaveChangesAsync();
        }

        public async Task ClearAllActivityAsync()
        {
            var allRecords = await _context.BorrowRecords.ToListAsync();
            _context.BorrowRecords.RemoveRange(allRecords);
            await _context.SaveChangesAsync();
        }
    }
}