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

        public void ProcessReturn(int recordId)
        {
            var record = _borrowRepo.GetRecordById(recordId);
            if (record != null)
            {
                var item = _inventoryRepo.GetAll().FirstOrDefault(i => i.Name == record.ItemName);
                if (item != null) item.Status = "Available";

                record.Status = "Returned";
                record.ReturnDate = DateTime.Now;

                _inventoryRepo.Update(item);
                _borrowRepo.UpdateRecord(record);
            }
        }
    }
}