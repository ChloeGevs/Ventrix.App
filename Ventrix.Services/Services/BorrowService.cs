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
            // IMPORTANT: The .Include(b => b.Borrower) tells Entity Framework to 
            // automatically grab the User's profile data (First Name, Last Name) 
            // attached to this ID so the Dashboard can display it!
            return await _context.BorrowRecords
                .Include(b => b.Borrower)
                .ToListAsync();
        }

        public async Task ProcessBorrowAsync(BorrowRecord record, int specificItemId)
        {
            // Ensure we map the ID directly to the model property
            record.InventoryItemId = specificItemId;
            record.BorrowDate = DateTime.Now;
            record.Status = BorrowStatus.Active;

            var item = await _context.InventoryItems.FindAsync(specificItemId);
            if (item != null)
            {
                item.Status = ItemStatus.Borrowed;
            }

            _context.BorrowRecords.Add(record);
            await _context.SaveChangesAsync();
        }

        public async Task ReturnItemAsync(int recordId)
        {
            // 1. Find the active borrow record
            var record = await _context.BorrowRecords.FindAsync(recordId);
            if (record != null)
            {
                // 2. Capture the EXACT time they clicked return
                record.ReturnDate = DateTime.Now;
                record.Status = BorrowStatus.Returned;

                // 3. Find the physical unit and make it Available again
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