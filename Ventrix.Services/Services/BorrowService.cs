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

        // Used when an item is confirmed to be damaged upon return
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

        // --- NEW: Manually force an item to Overdue status (Replaces old MarkAsOverdueAsync) ---
        public async Task ManuallyMarkOverdueAsync(List<int> recordIds, UserService userService)
        {
            var records = await _context.BorrowRecords.Where(b => recordIds.Contains(b.Id)).ToListAsync();

            foreach (var record in records)
            {
                if (record.Status == BorrowStatus.Active)
                {
                    record.Status = BorrowStatus.Overdue;
                    // Automatically applies a strike to the borrower
                    await userService.AddStrikeAsync(record.BorrowerId);
                }
            }
            await _context.SaveChangesAsync();
        }

        // --- NEW: Force return items without borrower request ---
        public async Task ForceReturnItemsAsync(List<int> recordIds)
        {
            var records = await _context.BorrowRecords.Where(b => recordIds.Contains(b.Id)).ToListAsync();

            foreach (var record in records)
            {
                if (record.Status == BorrowStatus.Active || record.Status == BorrowStatus.Overdue || record.Status == BorrowStatus.PendingReturn)
                {
                    record.Status = BorrowStatus.Returned;
                    record.ReturnDate = DateTime.Now;

                    // Make the physical item available again
                    var item = await _context.InventoryItems.FindAsync(record.InventoryItemId);
                    if (item != null)
                    {
                        item.Status = ItemStatus.Available;
                    }
                }
            }
            await _context.SaveChangesAsync();
        }

        // --- NEW: Automatically checks and penalizes overdue items ---
        public async Task<int> ProcessAutomaticOverdueStrikesAsync(UserService userService)
        {
            var activeRecords = await _context.BorrowRecords
                .Where(b => b.Status == BorrowStatus.Active)
                .ToListAsync();

            int newlyOverdueCount = 0;

            foreach (var record in activeRecords)
            {
                // If the current time is greater than the borrow date + 24 hours
                if (DateTime.Now > record.BorrowDate.AddDays(1))
                {
                    record.Status = BorrowStatus.Overdue;
                    await userService.AddStrikeAsync(record.BorrowerId);
                    newlyOverdueCount++;
                }
            }

            if (newlyOverdueCount > 0)
            {
                await _context.SaveChangesAsync();
            }

            return newlyOverdueCount;
        }

        // --- NEW: Hides a single record from the dashboard ---
        public async Task HideRecordFromDashboardAsync(int recordId)
        {
            var record = await _context.BorrowRecords.FindAsync(recordId);
            if (record != null)
            {
                record.IsHiddenFromDashboard = true;
                await _context.SaveChangesAsync();
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

        // --- NEW: Restores all hidden activities ---
        public async Task RestoreHiddenActivitiesAsync()
        {
            var hiddenRecords = await _context.BorrowRecords
                .Where(b => b.IsHiddenFromDashboard == true)
                .ToListAsync();

            foreach (var record in hiddenRecords)
            {
                record.IsHiddenFromDashboard = false;
            }

            await _context.SaveChangesAsync();
        }
    }
}