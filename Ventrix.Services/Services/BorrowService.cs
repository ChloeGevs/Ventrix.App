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
    public class BorrowService
    {
        private readonly AppDbContext _context;

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

        public async Task AddBorrowRecordAsync(int userId, int itemId, int quantity, string purpose, GradeLevel gradeLevel)
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
            // 1. CLEAN THE INPUT: Remove accidental spaces and force it to UPPERCASE
            string cleanBorrowerId = record.BorrowerId?.Trim().ToUpper() ?? "";

            // 2. SEARCH SMARTLY: Compare it against the database in uppercase
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId.ToUpper() == cleanBorrowerId);

            if (user == null)
            {
                // Added the ID they typed into the error message so they can see if they made a typo!
                throw new Exception($"Borrower ID '{record.BorrowerId}' not found in the system. Please check your ID and try again.");
            }

            if (user.Role == UserRole.Student && record.Quantity > 2)
            {
                throw new Exception($"Borrow limit exceeded. Students are allowed up to 2 items.");
            }
            else if (user.Role == UserRole.Faculty && record.Quantity > 10)
            {
                throw new Exception($"Borrow limit exceeded. Faculty members are allowed up to 10 items.");
            }

            // Map the User's Primary Key (Id) to the Record's Foreign Key (UserId)
            record.UserId = user.Id;
            record.InventoryItemId = itemId;
            record.BorrowDate = DateTime.Now;
            

            var item = await _context.InventoryItems.FindAsync(itemId);
            if (item != null)
            {
                item.Status = ItemStatus.Borrowed;
                _context.InventoryItems.Update(item);
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