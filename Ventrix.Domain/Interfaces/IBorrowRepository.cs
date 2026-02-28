using Ventrix.Domain.Models;

namespace Ventrix.Domain.Interfaces
{
    public interface IBorrowRepository
    {
        void AddRecord(BorrowRecord record);
        BorrowRecord GetRecordById(int id);
        void UpdateRecord(BorrowRecord record);
        IEnumerable<BorrowRecord> GetAll();
        IEnumerable<BorrowRecord> GetActiveRecords();
        IEnumerable<BorrowRecord> GetReturnedHistory();
    }
}