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
            var user = await _context.Users.FindAsync(record.BorrowerId);
            if (user == null) throw new Exception("User not found.");

            if (user.Role == UserRole.Student)
            {
                if (user.IsLockedOut) throw new Exception("Student account is locked due to strikes.");

                var activeCount = await _context.BorrowRecords.CountAsync(b =>
                    b.BorrowerId == record.BorrowerId &&
                    (b.Status == BorrowStatus.Active || b.Status == BorrowStatus.Pending || b.Status == BorrowStatus.Overdue));

                if (activeCount >= 3) throw new Exception("Student has reached the maximum limit of 3 items.");
            }

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
                var item = await _context.InventoryItems.FindAsync(record.InventoryItemId);

                if (item == null || item.Status != ItemStatus.Available)
                {
                    throw new Exception("The specific item unit is no longer available for approval.");
                }

                record.Status = BorrowStatus.Active;
                record.BorrowDate = DateTime.Now;
                item.Status = ItemStatus.Borrowed;

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

        // FIX: Changed the first parameter to accept List<int> recordIds instead of List<BorrowRecord>
        public async Task ManuallyMarkOverdueAsync(List<int> recordIds, UserService userService)
        {
            if (recordIds == null || !recordIds.Any()) return;

            var recordsToUpdate = await _context.BorrowRecords
                .Where(r => recordIds.Contains(r.Id))
                .ToListAsync();

            // Group all the selected items by the student who borrowed them
            var groupedByBorrower = recordsToUpdate.GroupBy(r => r.BorrowerId);

            foreach (var group in groupedByBorrower)
            {
                string borrowerId = group.Key;

                // --- ITEM LOOP STARTS ---
                // We loop through the items ONLY to change their status
                foreach (var record in group)
                {
                    record.Status = BorrowStatus.Overdue;
                }
                // --- ITEM LOOP ENDS ---

                // CRITICAL: The strike MUST be outside the item loop!
                // Because it is out here, it only happens ONCE per student, 
                // no matter how many items were in the loop above.
                await userService.AddStrikeAsync(borrowerId);
            }

            await _context.SaveChangesAsync();
        }

        public async Task ForceReturnItemsAsync(List<int> recordIds)
        {
            var records = await _context.BorrowRecords.Where(b => recordIds.Contains(b.Id)).ToListAsync();

            foreach (var record in records)
            {
                if (record.Status == BorrowStatus.Active || record.Status == BorrowStatus.Overdue || record.Status == BorrowStatus.PendingReturn)
                {
                    record.Status = BorrowStatus.Returned;
                    record.ReturnDate = DateTime.Now;

                    var item = await _context.InventoryItems.FindAsync(record.InventoryItemId);
                    if (item != null)
                    {
                        item.Status = ItemStatus.Available;
                    }
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task<int> ProcessAutomaticOverdueStrikesAsync(UserService userService)
        {
            var overdueRecords = await _context.BorrowRecords
                .Where(b => b.Status == BorrowStatus.Active && DateTime.Now > b.BorrowDate.AddDays(7))
                .ToListAsync();

            if (!overdueRecords.Any()) return 0;

            // Group the late items by student
            var overdueGroups = overdueRecords.GroupBy(b => b.BorrowerId);

            foreach (var group in overdueGroups)
            {
                string borrowerId = group.Key;

                // Change statuses
                foreach (var record in group)
                {
                    record.Status = BorrowStatus.Overdue;
                }

                // CRITICAL: Give 1 strike per student, outside the item loop!
                await userService.AddStrikeAsync(borrowerId);
            }

            await _context.SaveChangesAsync();
            return overdueRecords.Count;
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