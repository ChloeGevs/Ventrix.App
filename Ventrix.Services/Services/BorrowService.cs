using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ventrix.Domain.Models;
using Ventrix.Domain.Enums;
using Ventrix.Infrastructure.Data;

namespace Ventrix.Application.Services
{
    public class BorrowService
    {
        private readonly AppDbContext _context;

        public BorrowService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BorrowRecord>> GetAllBorrowRecordsAsync()
        {
            return await _context.BorrowRecords
                .Include(b => b.Borrower)
                .ToListAsync();
        }

        public async Task ProcessBorrowAsync(BorrowRecord record, int specificItemId)
        {
            record.InventoryItemId = specificItemId;
            record.BorrowDate = DateTime.Now;
            record.Status = BorrowStatus.Pending;

            _context.BorrowRecords.Add(record);
            await _context.SaveChangesAsync();
        }

        public async Task ApproveBorrowAsync(int recordId)
        {
            var record = await _context.BorrowRecords.FindAsync(recordId);

            if (record != null && record.Status == BorrowStatus.Pending)
            {
                record.Status = BorrowStatus.Active;
                record.BorrowDate = DateTime.Now;

                var item = await _context.InventoryItems.FindAsync(record.InventoryItemId);
                if (item != null)
                {
                    item.Status = ItemStatus.Borrowed;
                }

                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Borrow record not found or is no longer pending.");
            }
        }

        public async Task RequestReturnAsync(int recordId)
        {
            var record = await _context.BorrowRecords.FindAsync(recordId);
            if (record != null && (record.Status == BorrowStatus.Active || record.Status == BorrowStatus.Overdue))
            {
                record.Status = BorrowStatus.PendingReturn;
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Borrow record not found or cannot be returned.");
            }
        }

        public async Task ReturnItemAsync(int recordId)
        {
            var record = await _context.BorrowRecords.FindAsync(recordId);
            if (record != null)
            {
                record.ReturnDate = DateTime.Now;
                record.Status = BorrowStatus.Returned;

                var item = await _context.InventoryItems.FindAsync(record.InventoryItemId);

                if (item != null)
                {
                    item.Status = ItemStatus.Available;
                }

                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Borrow record not found in the database.");
            }
        }

        // NEW METHOD: Used when an item is confirmed to be damaged upon return
        public async Task ReturnItemAsDamagedAsync(int recordId)
        {
            var record = await _context.BorrowRecords.FindAsync(recordId);
            if (record != null)
            {
                record.ReturnDate = DateTime.Now;
                record.Status = BorrowStatus.Returned;

                var item = await _context.InventoryItems.FindAsync(record.InventoryItemId);

                if (item != null)
                {
                    item.Status = ItemStatus.Available;
                    item.Condition = Condition.Damaged; // Specifically mark the physical condition as Damaged
                }

                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Borrow record not found in the database.");
            }
        }

        public async Task MarkAsOverdueAsync(int recordId)
        {
            var record = await _context.BorrowRecords.FindAsync(recordId);

            if (record != null && record.Status == BorrowStatus.Active)
            {
                record.Status = BorrowStatus.Overdue;
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Record not found or is not currently Active.");
            }
        }

        public async Task ClearAllActivityAsync()
        {
            var visibleRecords = await _context.BorrowRecords
                .Where(b => b.IsHiddenFromDashboard == false)
                .ToListAsync();

            foreach (var record in visibleRecords)
            {
                record.IsHiddenFromDashboard = true;

            }
            await _context.SaveChangesAsync();
        }
    }
}