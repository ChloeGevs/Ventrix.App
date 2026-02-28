using System;
using System.Collections.Generic;
using System.Linq;
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
        public void ProcessBorrow(BorrowRecord record, int itemId)
        {
            var item = _inventoryRepo.GetById(itemId);
            if (item != null && item.Status == "Available")
            {
                item.Status = "Borrowed";
                _inventoryRepo.Update(item);
                _borrowRepo.AddRecord(record);
            }
        }

        // --- READ ---
        public IEnumerable<BorrowRecord> GetAllBorrowRecords()
        {
            return _borrowRepo.GetAll();
        }

        // --- UPDATE (RETURN) ---
        // Consolidating ProcessReturn and ReturnItem into one robust method
        public bool ReturnItem(int recordId)
        {
            var record = _borrowRepo.GetRecordById(recordId);
            if (record == null) return false;

            // Find the item associated with this record
            var item = _inventoryRepo.GetAll().FirstOrDefault(i => i.Name == record.ItemName);

            if (item != null)
            {
                item.Status = "Available";
                _inventoryRepo.Update(item);
            }

            // Update the record status
            record.Status = "Returned";
            record.ReturnDate = DateTime.Now;
            _borrowRepo.UpdateRecord(record);

            return true;
        }
    }
}