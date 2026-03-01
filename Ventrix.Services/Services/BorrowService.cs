using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; // Required for Task
using Ventrix.Domain.Interfaces;
using Ventrix.Domain.Models;

namespace Ventrix.Application.Services
{
    public class BorrowService
    {
        private readonly IBorrowRepository _borrowRepo;
        private readonly IInventoryRepository _inventoryRepo;

        public BorrowService(IBorrowRepository borrowRepo, IInventoryRepository inventoryRepo)
        {
            _borrowRepo = borrowRepo;
            _inventoryRepo = inventoryRepo;
        }

        // --- CREATE ---
        public async Task ProcessBorrowAsync(BorrowRecord record, int itemId)
        {
            var item = await _inventoryRepo.GetByIdAsync(itemId);
            if (item != null && item.Status == "Available")
            {
                item.Status = "Borrowed";
                await _inventoryRepo.UpdateAsync(item);
                await _borrowRepo.AddRecordAsync(record);
            }
        }

        // --- READ ---
        public async Task<IEnumerable<BorrowRecord>> GetAllBorrowRecordsAsync()
        {
            return await _borrowRepo.GetAllAsync();
        }

        // --- UPDATE (RETURN) ---
        public async Task<bool> ReturnItemAsync(int recordId)
        {
            var record = await _borrowRepo.GetRecordByIdAsync(recordId);
            if (record == null) return false;

            // Find the item associated with this record
            var items = await _inventoryRepo.GetAllAsync();
            var item = items.FirstOrDefault(i => i.Name == record.ItemName);

            if (item != null)
            {
                item.Status = "Available";
                await _inventoryRepo.UpdateAsync(item);
            }

            // Update the record status
            record.Status = "Returned";
            record.ReturnDate = DateTime.Now;
            await _borrowRepo.UpdateRecordAsync(record);

            return true;
        }

        public async Task ClearAllActivityAsync()
        {
            await _borrowRepo.ClearAllRecordsAsync();
        }
    }
}